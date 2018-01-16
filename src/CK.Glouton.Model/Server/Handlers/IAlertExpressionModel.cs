namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertExpressionModel
    {
        IExpressionModel[] Expressions { get; set; }
        string[] Senders { get; set; }
    }
}