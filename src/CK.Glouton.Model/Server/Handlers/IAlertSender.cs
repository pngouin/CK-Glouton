using CK.Monitoring;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertSender
    {
        void Send( IAlertEntry logEntry );
    }
}