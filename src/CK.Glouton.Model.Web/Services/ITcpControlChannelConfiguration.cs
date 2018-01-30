using System.Collections.Generic;

namespace CK.Glouton.Model.Web.Services
{
    public interface ITcpControlChannelConfiguration
    {
        Dictionary<string, string> AdditionalAuthenticationData { get; set; }
        string AppName { get; set; }
        string ClientName { get; set; }
        int ConnectionRetryDelayMs { get; set; }
        bool HandleSystemActivityMonitorErrors { get; set; }
        string Host { get; set; }
        bool IsSecure { get; set; }
        int Port { get; set; }
        bool PresentEnvironmentVariables { get; set; }
        bool PresentMonitoringAssemblyInformation { get; set; }

        IReadOnlyDictionary<string, string> BuildAuthData();
    }
}