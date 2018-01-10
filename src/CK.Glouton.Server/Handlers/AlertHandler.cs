using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandler : IGloutonHandler
    {
        private AlertHandlerConfiguration _alertHandlerConfiguration;
        private IActivityMonitor _activityMonitor;

        public AlertHandler( AlertHandlerConfiguration alertHandlerConfiguration )
        {
            _alertHandlerConfiguration = alertHandlerConfiguration;
        }

        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            foreach( var (condition, sender) in _alertHandlerConfiguration.Alerts )
            {
                if( !condition( receivedData ) )
                    continue;

                _activityMonitor.Info( "An alert has been sent" );
                sender.Send( receivedData );
            }
        }

        public void OnTimer( IActivityMonitor activityMonitor, TimeSpan timerDuration )
        {
        }

        public bool ApplyConfiguration( IGloutonHandlerConfiguration configuration )
        {
            return configuration is AlertHandlerConfiguration alertHandlerConfiguration
                && new HashSet<(Func<ReceivedData, bool> condition, IAlertSender sender)>( _alertHandlerConfiguration.Alerts )
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