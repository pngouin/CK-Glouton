using CK.Glouton.Model.Server.Handlers.Implementation;

namespace CK.Glouton.Model.Server.Sender
{
    public interface IAlertSender
    {
        string SenderType { get; set; }
        bool Match( IAlertSenderConfiguration configuration );
        void Send( AlertEntry logEntry );
    }
}