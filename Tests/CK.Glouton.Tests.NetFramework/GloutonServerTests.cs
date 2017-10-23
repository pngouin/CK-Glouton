using CK.Core;
using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Text;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class GloutonServerTests
    {
        [SetUp]
        public void Setup()
        {
            if( !System.Console.IsOutputRedirected )
                System.Console.OutputEncoding = Encoding.UTF8;

            LogFile.RootLogPath = TestHelper.GetTestLogDirectory();

            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );

            var grandOutputConfigurationServer = new GrandOutputConfiguration();
            grandOutputConfigurationServer.AddHandler( new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfigurationServer );

            var grandOutputConfigurationClient = new GrandOutputConfiguration();
            grandOutputConfigurationClient.AddHandler( new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            } );
            grandOutputConfigurationClient.AddHandler( new TcpHandlerConfiguration
            {
                Host = TestHelper.DefaultHost,
                Port = TestHelper.DefaultPort,
                IsSecure = false,
                AppName = typeof( TestHelper ).GetTypeInfo().Assembly.GetName().Name,
                PresentEnvironmentVariables = true,
                PresentMonitoringAssemblyInformation = true,
                HandleSystemActivityMonitorErrors = true
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfigurationClient );
        }


        [TearDown]
        public void TearDown()
        {
            GrandOutput.Default.Dispose();
        }

        [Test]
        public void server_can_be_open()
        {
            using( var server = TestHelper.DefaultServer() )
            {
                server.Should().NotBeNull();
                Action open = () => server.Open();
                open.ShouldNotThrow();
            }
        }
    }
}