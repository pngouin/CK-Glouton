using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System.Reflection;

namespace CK.Glouton.Tests
{
    internal static class GrandOutputHelper
    {
        private static GrandOutput _grandOutputServer;
        private static GrandOutput _grandOutputClient;

        internal static GrandOutput GrandOutputServer => _grandOutputServer ?? ( _grandOutputServer = InitializeGrandOutputServer() );

        internal static GrandOutput GrandOutputClient => _grandOutputClient ?? ( _grandOutputClient = InitializeGrandOutputClient() );

        private static GrandOutput InitializeGrandOutputServer()
        {
            var textFileConfiguration = new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            };

            return new GrandOutput( new GrandOutputConfiguration { Handlers = { textFileConfiguration } } );
        }

        private static GrandOutput InitializeGrandOutputClient()
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

        internal static void DisposeGrandOutputs()
        {
            _grandOutputServer?.Dispose();
            _grandOutputClient?.Dispose();
        }
    }
}
