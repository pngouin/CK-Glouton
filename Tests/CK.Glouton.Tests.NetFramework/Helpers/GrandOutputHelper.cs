using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System.Reflection;

namespace CK.Glouton.Tests
{
    internal static class GrandOutputHelper
    {
        internal static GrandOutput GetGrandOutputServer()
        {
            var textFileConfiguration = new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            };

            return new GrandOutput( new GrandOutputConfiguration { Handlers = { textFileConfiguration } } );
        }

        internal static GrandOutput GetGrandOutputClient()
        {
            var textFileConfiguration = new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            };
            var tcpHandlerConfiguration = new TcpHandlerConfiguration
            {
                Host = TestHelper.DefaultHost,
                Port = TestHelper.DefaultPort,
                IsSecure = false,
                AppName = typeof( TestHelper ).GetTypeInfo().Assembly.GetName().Name,
                PresentEnvironmentVariables = true,
                PresentMonitoringAssemblyInformation = true,
                HandleSystemActivityMonitorErrors = true
            };

            return new GrandOutput( new GrandOutputConfiguration { Handlers = { textFileConfiguration, tcpHandlerConfiguration } } );
        }
    }
}
