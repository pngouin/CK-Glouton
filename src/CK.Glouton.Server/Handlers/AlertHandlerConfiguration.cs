using CK.Glouton.Model.Server;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandlerConfiguration : IGloutonHandlerConfiguration
    {
        public List<(Func<ReceivedData, bool> condition, IAlertSender sender)> Alerts { get; set; }

        public IGloutonHandlerConfiguration Clone()
        {
            return new AlertHandlerConfiguration
            {
                Alerts = Alerts
            };
        }
    }
}