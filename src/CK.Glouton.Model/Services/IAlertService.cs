using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Services
{
    public interface IAlertService
    {
        bool SendNewAlert( AlertExpressionModel alertExpression );
        IMailConfiguration GetMailConfiguration();
        string[] AvailableConfiguration { get; }
        IHttpConfiguration GetHttpConfiguration();
    }
}