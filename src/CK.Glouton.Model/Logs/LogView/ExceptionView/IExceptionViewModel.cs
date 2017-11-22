namespace CK.Glouton.Model
{
    public interface IExceptionViewModel
    {
        IInnerExceptionViewModel InnerException { get; set; }
        string Message { get; set; }
        string Stack { get; set; }
    }
}