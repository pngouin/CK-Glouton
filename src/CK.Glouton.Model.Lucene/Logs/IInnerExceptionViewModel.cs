namespace CK.Glouton.Model.Lucene.Logs
{
    public interface IInnerExceptionViewModel
    {
        string StackTrace { get; set; }
        string Message { get; set; }
        string FileName { get; set; }
    }
}