namespace CK.Glouton.Model.Server.Sender
{
    public interface IAlertSenderConfiguration
    {
        string SenderType { get; set; }
        object Configuration { get; set; }
        IAlertSenderConfiguration Default();
        IAlertSenderConfiguration Clone();
    }
}