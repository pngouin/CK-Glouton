using CK.Monitoring;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IAlertEntry : IMulticastLogEntry
    {
        string AppName { get; set; }
    }
}
