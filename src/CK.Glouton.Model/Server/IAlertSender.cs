using CK.Monitoring;

namespace CK.Glouton.Model.Server
{
    public interface IAlertSender
    {
        void Send( ILogEntry logEntry );
    }
}