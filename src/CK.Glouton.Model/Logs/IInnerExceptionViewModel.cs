namespace CK.Glouton.Model.Logs
{
    public interface IInnerExceptionViewModel
    {
        string StackTrace { get; set; }
        string Message { get; set; }
        string FileName { get; set; }
    }
}