using CK.Glouton.Model.Server.Handlers;

namespace CK.Glouton.Model.Services
{
    public interface IAlertService
    {
        bool SendNewAlert( IAlertExpressionModel alertExpression );
    }
}