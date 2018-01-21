using System.Collections.Generic;
using CK.Glouton.Model.Server.Handlers;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandlerConfiguration : IGloutonHandlerConfiguration
    {
        public List<IAlertModel> Alerts { get; set; }

        public IGloutonHandlerConfiguration Clone()
        {
            return new AlertHandlerConfiguration
            {
                Alerts = Alerts
            };
        }
    }
}