using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System.Reflection;

namespace CK.Glouton.Tests
{
    internal static class GrandOutputHelper
    {
        /// <summary>
        /// Returns a new Grand Output Server.
        /// It only contains a <see cref="TextFileConfiguration"/>.
        /// </summary>
        /// <returns></returns>
        internal static GrandOutput GetNewGrandOutputServer()
        {
            var textFileConfiguration = new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            };

            return new GrandOutput( new GrandOutputConfiguration { Handlers = { textFileConfiguration } } );
        }

        /// <summary>
        /// Returns a new Grand Output Client.
        /// It contains a <see cref="TextFileConfiguration"/> and also a <see cref="TcpHandlerConfiguration"/>.
        /// The tcp handler will use <see cref="TestHelper.DefaultHost"/> and <see cref="TestHelper.DefaultPort"/> from <see cref="TestHelper"/>.
        /// </summary>
        /// <returns></returns>
        internal static GrandOutput GetNewGrandOutputClient()
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
