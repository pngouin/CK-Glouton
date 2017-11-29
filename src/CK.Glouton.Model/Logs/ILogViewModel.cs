namespace CK.Glouton.Model.Logs
{
    public interface ILogViewModel
    {
        ELogType LogType { get; }
        IExceptionViewModel Exception { get; set; }
        string LogTime { get; set; }
        string LogLevel { get; set; }
    }
}
