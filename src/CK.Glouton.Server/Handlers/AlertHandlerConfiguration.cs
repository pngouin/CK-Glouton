using CK.Glouton.Model.Server;
using CK.Monitoring;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandlerConfiguration : IGloutonHandlerConfiguration
    {
        public List<(Func<ILogEntry, bool> condition, IList<IAlertSender> senders)> Alerts { get; set; }

        public IGloutonHandlerConfiguration Clone()
        {
            return new AlertHandlerConfiguration
            {
                Alerts = Alerts
            };
        }
    }
}