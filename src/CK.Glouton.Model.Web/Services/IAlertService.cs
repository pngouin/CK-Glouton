using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using System.Collections.Generic;

namespace CK.Glouton.Model.Web.Services
{
    public interface IAlertService
    {
        string[] AvailableConfiguration { get; }
        bool TryGetConfiguration( IActivityMonitor activityMonitor, string key, out IAlertSenderConfiguration configuration );
        bool NewAlertRequest( IActivityMonitor activityMonitor, AlertExpressionModel alertExpression );
        IList<IAlertExpressionModel> GetAllAlerts();
    }
}