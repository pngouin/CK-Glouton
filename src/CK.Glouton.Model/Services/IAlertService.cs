using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Services
{
    public interface IAlertService
    {
        bool SendNewAlert( IAlertExpressionModel alertExpression );
        IMailConfiguration GetMailConfiguration();
        string[] AvailableConfiguration { get; }
    }
}