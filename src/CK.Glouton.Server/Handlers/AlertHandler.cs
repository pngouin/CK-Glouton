using CK.Core;
using CK.Glouton.Model.Server;
using CK.Glouton.Model.Server.Handlers;
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
            if( _alertHandlerConfiguration.Alerts == null )
                _alertHandlerConfiguration.Alerts = new List<(Func<ILogEntry, bool>, IList<IAlertSender>)>();
        }

        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            var version = Convert.ToInt32( receivedData.ServerClientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( receivedData.Data.ToArray(), 0, receivedData.Data.Count );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );

            List<(Func<ILogEntry, bool> condition, IList<IAlertSender> senders)> faulty = null;

            foreach( var alert in _alertHandlerConfiguration.Alerts )
            {
                try
                {
                    if( !alert.condition( entry ) )
                        continue;

                    _activityMonitor.Info( "An alert has been sent" );
                    foreach( var sender in alert.senders )
                        sender.Send( entry );
                }
                catch( Exception exception )
                {
                    const string message = "Alert crashed.";
                    ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                    _activityMonitor.Fatal( message, exception );
                    if( faulty == null )
                        faulty = new List<(Func<ILogEntry, bool> condition, IList<IAlertSender> senders)>();
                    faulty.Add( alert );
                }
            }

            if( faulty == null )
                return;

            foreach( var faultyAlert in faulty )
                _alertHandlerConfiguration.Alerts.Remove( faultyAlert );
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