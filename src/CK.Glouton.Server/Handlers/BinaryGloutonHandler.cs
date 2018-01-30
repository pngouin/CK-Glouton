using CK.Core;
using CK.Glouton.Model.Server;
using CK.Glouton.Model.Server.Handlers;
using CK.Monitoring;
using System;
using System.IO;
using System.Text;

namespace CK.Glouton.Server.Handlers
{
    public class BinaryGloutonHandler : IGloutonHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;

        private MonitorBinaryFileOutput _file;
        private BinaryGloutonHandlerConfiguration _configuration;

        public BinaryGloutonHandler( BinaryGloutonHandlerConfiguration configuration )
        {
            if( configuration == null )
                throw new ArgumentNullException( nameof( configuration ) );

            _file = new MonitorBinaryFileOutput( configuration.Path, configuration.MaxCountPerFile, configuration.UseGzipCompression );
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );

            _configuration = configuration;
        }

        /// <summary>
        /// Writes log in the binary file.
        /// </summary>
        /// <param name="receivedData"></param>
        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            var version = Convert.ToInt32( receivedData.ServerClientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( receivedData.Data.ToArray(), 0, receivedData.Data.Count );
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            _file.Write( LogEntry.Read( _binaryReader, version, out _ ) );
        }

        public void OnTimer( IActivityMonitor activityMonitor, TimeSpan timerDuration )
        {
        }

        public bool ApplyConfiguration( IGloutonHandlerConfiguration configuration )
        {
            if( !( configuration is BinaryGloutonHandlerConfiguration cF ) || cF.Path != _configuration.Path )
                return false;
            if( _configuration.UseGzipCompression != cF.UseGzipCompression )
            {
                _file.Close();
                _file = new MonitorBinaryFileOutput( _configuration.Path, _configuration.MaxCountPerFile, _configuration.UseGzipCompression );
            }
            else
                _file.MaxCountPerFile = cF.MaxCountPerFile;
            _configuration = cF;
            return true;
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
