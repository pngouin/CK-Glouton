namespace CK.Glouton.Model.Server.Handlers
{
    public interface IExpressionModel
    {
        string Field { get; set; }
        string Operation { get; set; }
        string Body { get; set; }
    }
}