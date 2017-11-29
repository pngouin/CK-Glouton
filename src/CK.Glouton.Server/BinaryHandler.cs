using CK.ControlChannel.Abstractions;
using CK.Core;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Text;

namespace CK.Glouton.Server
{
    public class BinaryHandler : IGloutonServerHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly MonitorBinaryFileOutput _file;

        public BinaryHandler( BinaryFileConfiguration config )
        {
            if( config == null )
                throw new ArgumentNullException( nameof( config ) );
            _file = new MonitorBinaryFileOutput( config.Path, config.MaxCountPerFile, config.UseGzipCompression );
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
        }

        /// <summary>
        /// Writes log in the binary file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clientSession"></param>
        public void OnGrandOutputEventInfo( byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] as string );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            _file.Write( LogEntry.Read( _binaryReader, version, out _ ) );


        }

        /// <summary>
        /// Initializes file and start the queues.
        /// </summary>
        public void Open( IActivityMonitor activityMonitor )
        {
            _file.Initialize( activityMonitor );
        }

        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            _file.Close();
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _disposedValue )
                return;

            Close();

            _disposedValue = true;
        }
        #endregion
    }
}
