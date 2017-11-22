namespace CK.Glouton.Model
{
    public interface IInnerExceptionViewModel
    {
        string Stack { get; set; }
        string Details { get; set; }
        string FileName { get; set; }
    }
}