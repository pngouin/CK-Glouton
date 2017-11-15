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

        internal static GrandOutput GrandOutputServer
        {
            get
            {
                if( _grandOutputServer == null )
                    InitializeGrandOutputServer();
                return _grandOutputServer;
            }
        }

        internal static GrandOutput GrandOutputClient
        {
            get
            {
                if( _grandOutputClient == null )
                    InitializeGrandOutputClient();
                return _grandOutputClient;
            }
        }

        private static void InitializeGrandOutputServer()
        {
            var textFileConfiguration = new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            };

            var grandOutputServer = new GrandOutput
            (
                new GrandOutputConfiguration
                {
                    Handlers = { textFileConfiguration }
                },
                true
            );

            _grandOutputServer = grandOutputServer;
        }

        private static void InitializeGrandOutputClient()
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

            var grandOutputClient = new GrandOutput(
                new GrandOutputConfiguration
                {
                    Handlers = { textFileConfiguration, tcpHandlerConfiguration }
                }
            );

            _grandOutputClient = grandOutputClient;
        }

        internal static void DisposeGrandOutputs()
        {
            _grandOutputServer?.Dispose();
            _grandOutputClient?.Dispose();
        }
    }
}
