using CK.Core;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandler : IGloutonHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;

        private readonly AlertHandlerConfiguration _alertHandlerConfiguration;

        private IActivityMonitor _activityMonitor;

        public AlertHandler( AlertHandlerConfiguration alertHandlerConfiguration )
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _alertHandlerConfiguration = alertHandlerConfiguration;
        }

        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            var version = Convert.ToInt32( receivedData.ServerClientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( receivedData.Data.ToArray(), 0, receivedData.Data.Count );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );

            foreach( var (condition, senders) in _alertHandlerConfiguration.Alerts )
            {
                if( !condition( entry ) )
                    continue;

                _activityMonitor.Info( "An alert has been sent" );
                foreach( var sender in senders )
                    sender.Send( entry );
            }
        }

        public void OnTimer( IActivityMonitor activityMonitor, TimeSpan timerDuration )
        {
        }

        public bool ApplyConfiguration( IGloutonHandlerConfiguration configuration )
        {
            return configuration is AlertHandlerConfiguration alertHandlerConfiguration
                && new HashSet<(Func<ILogEntry, bool> condition, IList<IAlertSender> senders)>( _alertHandlerConfiguration.Alerts )
                    .SetEquals( alertHandlerConfiguration.Alerts );
        }

        public void Open( IActivityMonitor activityMonitor )
        {
            _activityMonitor = activityMonitor;
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}