using CK.ControlChannel.Abstractions;
using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Server
{
    public class LuceneHandler : IGloutonServerHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _blockingQueue;
        private readonly Dictionary<string, LuceneIndexer> _indexerDictionary;

        private Task _blockingQueueThread;
        private bool _isDisposing;

        public LuceneHandler()
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _blockingQueue = new ConcurrentQueue<Action>();
            _indexerDictionary = new Dictionary<string, LuceneIndexer>();
        }

        /// <summary>
        /// Sends log into the queue to be indexed.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clientSession"></param>
        public void OnGrandOutputEventInfo( byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] as string );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );
            clientSession.ClientData.TryGetValue( "AppName", out var appName );
            var clientData = clientSession.ClientData as IReadOnlyDictionary<string, string>;


            if( _indexerDictionary.ContainsKey( appName ) )
            {
                _indexerDictionary.TryGetValue( appName, out var indexer );
                _blockingQueue.Enqueue( () => indexer.IndexLog( entry, clientData ) );
            }
            else
            {
                var indexer = new LuceneIndexer( appName );
                _indexerDictionary.Add( appName, indexer );
                _blockingQueue.Enqueue( () => indexer.IndexLog( entry, clientData ) );
            }
        }

        private void DisposeAllIndexer()
        {
            foreach( var entry in _indexerDictionary )
                entry.Value.Dispose();
        }

        private void ProcessQueue( ConcurrentQueue<Action> queue )
        {
            while( !queue.IsEmpty || !_isDisposing )
            {
                queue.TryDequeue( out var action );
                action?.Invoke();
            }
        }

        /// <summary>
        /// Starts queues.
        /// </summary>
        public void Open( IActivityMonitor activityMonitor )
        {
            _blockingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _blockingQueue ) );
        }

        /// <summary>
        /// Closes what need to be closed.
        /// </summary>
        public void Close()
        {
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _isDisposing || _disposedValue )
                return;

            _isDisposing = true;

            Close();
            System.Threading.SpinWait.SpinUntil( () => _blockingQueueThread.IsCompleted );

            _blockingQueueThread.Dispose();
            DisposeAllIndexer();

            _disposedValue = true;
        }

        #endregion
    }
}
