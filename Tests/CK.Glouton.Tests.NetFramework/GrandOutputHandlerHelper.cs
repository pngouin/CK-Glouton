using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Tests
{
    public class GrandOutputHandlerHelper 
    {
        public static void SetupHandler()
        {
            var grandOutputConfigurationClient = new GrandOutputConfiguration();
            grandOutputConfigurationClient.AddHandler(new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            });
            grandOutputConfigurationClient.AddHandler(new TcpHandlerConfiguration
            {
                Host = TestHelper.DefaultHost,
                Port = TestHelper.DefaultPort,
                IsSecure = false,
                AppName = typeof(TestHelper).GetTypeInfo().Assembly.GetName().Name,
                PresentEnvironmentVariables = true,
                PresentMonitoringAssemblyInformation = true,
                HandleSystemActivityMonitorErrors = true
            });
            GrandOutput.EnsureActiveDefault(grandOutputConfigurationClient);
        }

        public static void TearDown()
        {
            GrandOutput.Default.Dispose();
        }
    }
}
