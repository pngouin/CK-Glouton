namespace CK.Glouton.Model.Server.Sender
{
    public interface IAlertSenderConfiguration
    {
        string SenderType { get; set; }
        IAlertSenderConfiguration Default();
        IAlertSenderConfiguration Clone();
    }
}