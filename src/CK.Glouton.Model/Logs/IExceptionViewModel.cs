namespace CK.Glouton.Model.Logs
{
    public interface IExceptionViewModel
    {
        IInnerExceptionViewModel InnerException { get; set; }
        string Message { get; set; }
        string Stack { get; set; }
    }
}