using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using System.Collections.Generic;

namespace CK.Glouton.Model.Web.Services
{
    public interface IAlertService
    {
        bool NewAlertRequest( AlertExpressionModel alertExpression );
        string[] AvailableConfiguration { get; }
        bool TryGetConfiguration( string key, out IAlertSenderConfiguration configuration );
        IList<IAlertExpressionModel> GetAllAlerts();
    }
}