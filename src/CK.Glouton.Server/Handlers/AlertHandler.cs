using CK.Core;
using CK.Glouton.AlertSender;
using CK.Glouton.Model.Server;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Server.Handlers.Common;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandler : IGloutonHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly AlertSenderManager _alertAlertSenderManager;

        private IActivityMonitor _activityMonitor;
        private List<IAlertModel> _alerts;

        public AlertHandler( AlertHandlerConfiguration alertHandlerConfiguration )
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _alertAlertSenderManager = new AlertSenderManager();
            InitializeAlerts( alertHandlerConfiguration );
        }

        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            var version = Convert.ToInt32( receivedData.ServerClientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( receivedData.Data.ToArray(), 0, receivedData.Data.Count );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var logEntry = LogEntry.Read( _binaryReader, version, out _ ) as IMulticastLogEntry;
            receivedData.ServerClientSession.ClientData.TryGetValue( "AppName", out var appName );
            var alertEntry = new AlertEntry( logEntry, appName );

            List<IAlertModel> faulty = null;

            foreach( var alert in _alerts )
            {
                try
                {
                    if( !alert.Condition( alertEntry ) )
                        continue;

                    _activityMonitor.Info( "An alert has been sent" );
                    foreach( var sender in alert.Senders )
                        sender.Send( alertEntry );
                }
                catch( Exception exception )
                {
                    const string message = "Alert crashed.";
                    _activityMonitor.Fatal( message, exception );
                    if( faulty == null )
                        faulty = new List<IAlertModel>();
                    faulty.Add( alert );
                }
            }

            if( faulty == null )
                return;

            foreach( var faultyAlert in faulty )
                _alerts.Remove( faultyAlert );
        }

        public void OnTimer( IActivityMonitor activityMonitor, TimeSpan timerDuration )
        {
        }

        public bool ApplyConfiguration( IGloutonHandlerConfiguration configuration )
        {
            if( !( configuration is AlertHandlerConfiguration alertHandlerConfiguration ) )
                return false;

            return InitializeAlerts( alertHandlerConfiguration );
        }

        internal bool InitializeAlerts( AlertHandlerConfiguration configuration )
        {
            try
            {
                var submittedAlerts = new List<IAlertModel>();
                foreach( var alertExpressionModel in configuration.Alerts )
                {
                    var senders = alertExpressionModel.Senders
                        .Select( alertSenderConfiguration => _alertAlertSenderManager.Parse( alertSenderConfiguration ) )
                        .ToList();
                    submittedAlerts.Add( new AlertModel
                    {
                        Condition = alertExpressionModel.Expressions.Build(),
                        Senders = senders
                    } );
                }

                _alerts = submittedAlerts;
                return true;
            }
            catch( Exception exception )
            {
                const string message = "Alert initialization failed.";
                _activityMonitor.Fatal( message, exception );
                return false;
            }
        }

        internal class AlertModel : IAlertModel
        {
            public Func<AlertEntry, bool> Condition { get; set; }
            public IList<IAlertSender> Senders { get; set; }
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