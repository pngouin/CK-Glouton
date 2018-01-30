namespace CK.Glouton.Model.Server
{
    public interface IHandlersManagerSink
    {
        void Handle( ReceivedData receivedData );
    }
}