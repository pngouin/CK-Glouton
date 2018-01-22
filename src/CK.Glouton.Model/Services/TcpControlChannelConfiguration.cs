using CK.Core;
using CK.Monitoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text;

namespace CK.Glouton.Model.Services
{
    public class TcpControlChannelConfiguration : ITcpControlChannelConfiguration
    {
        public string Host { get; set; }

        /// <summary>
        /// Port of the CK.Monitoring.Tcp server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// True if the connection with the CK.Monitoring.Tcp server
        /// is secured with SSL/TLS; otherwise false.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// The minimum delay before trying to open a connection with the CK.Monitoring.Tcp server
        /// after a connection failed, in milliseconds.
        /// </summary>
        public int ConnectionRetryDelayMs { get; set; } = 10 * 1000;

        /// <summary>
        /// The name of the application, as presented to the CK.Monitoring.Tcp server.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// The name of the client, as presented to the CK.Monitoring.Tcp server.
        /// Defaults to the host name of the local computer.
        /// </summary>
        public string ClientName { get; set; } = Dns.GetHostName();

        /// <summary>
        /// Additional authentication data, as presented to the CK.Monitoring.Tcp server.
        /// </summary>
        public Dictionary<string, string> AdditionalAuthenticationData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// True if the authentication data presented to the CK.Monitoring.Tcp server
        /// should include assembly information about this assembly and CK.Monitoring's assembly.
        /// </summary>
        public bool PresentMonitoringAssemblyInformation { get; set; }

        /// <summary>
        /// True if the authentication data presented to the CK.Monitoring.Tcp server
        /// should include all environment variables available to the running process.
        /// </summary>
        public bool PresentEnvironmentVariables { get; set; }

        /// <summary>
        /// True if low-level <see cref="SystemActivityMonitor"/> errors should be sent.
        /// </summary>
        public bool HandleSystemActivityMonitorErrors { get; set; }

        public IReadOnlyDictionary<string, string> BuildAuthData()
        {
            var dictionary = new Dictionary<string, string>();
            if (AdditionalAuthenticationData != null)
                foreach (var kvp in AdditionalAuthenticationData)
                    dictionary.Add(kvp.Key, kvp.Value);

            dictionary["AppName"] = AppName;
            dictionary["ClientName"] = ClientName;

            if (PresentMonitoringAssemblyInformation)
            {
                AddAssemblyInformation(dictionary, typeof(TcpControlChannelConfiguration));
            }

            if (!PresentEnvironmentVariables)
                return dictionary;

            foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
                dictionary[$"ENV:{e.Key}"] = e.Value.ToString();

            return dictionary;
        }

        private static void AddAssemblyInformation(IDictionary<string, string> dictionary, Type type)
        {
            var assemblyName = type.GetTypeInfo().Assembly.GetName();
            dictionary[$"ASSEMBLY:{assemblyName.Name}"] = assemblyName.FullName;
        }
    }
}
