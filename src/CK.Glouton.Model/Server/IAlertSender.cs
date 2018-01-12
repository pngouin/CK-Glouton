namespace CK.Glouton.Model.Server
{
    public interface IAlertSender
    {
        void Send( ReceivedData receivedData );
    }
}