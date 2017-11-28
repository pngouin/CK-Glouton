using CK.ControlChannel.Abstractions;
using CK.Core;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Glouton.Server
{
    public class BinaryHandler : IGloutonServerHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _processingQueue;
        private readonly ConcurrentQueue<Action> _blockingQueue;
        private readonly MonitorBinaryFileOutput _file;

        private bool _isDisposing;
        private Task _processingQueueThread;
        private Task _blockingQueueThread;

        public BinaryHandler( BinaryFileConfiguration config )
        {
            if( config == null )
                throw new ArgumentNullException( "config" );
            _file = new MonitorBinaryFileOutput( config.Path, config.MaxCountPerFile, config.UseGzipCompression );
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _processingQueue = new ConcurrentQueue<Action>();
            _blockingQueue = new ConcurrentQueue<Action>();
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            _file.Close();
        }

        /// <summary>
        /// Writes log in the binary file.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="data"></param>
        /// <param name="clientSession"></param>
        public void OnGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            _processingQueue.Enqueue( () => ProcessData( data, clientSession ) );
        }

        /// <summary>
        /// Initializes file and start the queues.
        /// </summary>
        public void Open()
        {
            var monitor = new ActivityMonitor(); //TODO: add Monitoring with CK.Monitoring in the server project
            _file.Initialize( monitor );
            _processingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _processingQueue ) );
            _blockingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _blockingQueue ) );
        }

        private void ProcessData( byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            var entry = LogEntry.Read( _binaryReader, version, out _ );

            _blockingQueue.Enqueue( () => _file.Write( entry ) );
        }

        private void ProcessQueue( ConcurrentQueue<Action> queue )
        {
            while( !queue.IsEmpty || !_isDisposing )
            {
                queue.TryDequeue( out var action );
                action?.Invoke();
            }
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _isDisposing || _disposedValue )
                return;

            _isDisposing = true;
            Close();
            SpinWait.SpinUntil( () => _blockingQueueThread.IsCompleted && _processingQueueThread.IsCompleted );
            _blockingQueueThread.Dispose();
            _processingQueueThread.Dispose();

            _disposedValue = true;
        }
        #endregion
    }
}
