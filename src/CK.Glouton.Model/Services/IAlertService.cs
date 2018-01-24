using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Services
{
    public interface IAlertService
    {
        bool NewAlertRequest( AlertExpressionModel alertExpression );
        string[] AvailableConfiguration { get; }
        bool TryGetConfiguration( string key, out IAlertSenderConfiguration configuration );
    }
}