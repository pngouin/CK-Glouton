using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertExpressionModel
    {
        IExpressionModel[] Expressions { get; set; }
        IAlertSenderConfiguration[] Senders { get; set; }
    }
}