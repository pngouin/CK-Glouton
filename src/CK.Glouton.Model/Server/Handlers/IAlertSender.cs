namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertSender
    {
        void Send( AlertEntry logEntry );
    }
}