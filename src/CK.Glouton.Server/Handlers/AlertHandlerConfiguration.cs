using CK.Glouton.Model.Server.Handlers;
using System.Collections.Generic;

namespace CK.Glouton.Server.Handlers
{
    public class AlertHandlerConfiguration : IGloutonHandlerConfiguration
    {
        public List<IAlertExpressionModel> Alerts { get; set; }
        public string DatabasePath { get; set; }

        public IGloutonHandlerConfiguration Clone()
        {
            return new AlertHandlerConfiguration
            {
                Alerts = Alerts,
                DatabasePath = DatabasePath
            };
        }

        public AlertHandlerConfiguration()
        {
            Alerts = new List<IAlertExpressionModel>();
        }
    }
}