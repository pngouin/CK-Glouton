using CK.Core;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class ServerTests
    {
        [SetUp]
        public void Setup()
        {
            LogFile.RootLogPath = TestHelper.GetTestLogDirectory();

            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );

            var grandOutputConfig = new GrandOutputConfiguration();
            grandOutputConfig.AddHandler( new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfig );
        }

        [TearDown]
        public void TearDown()
        {
            GrandOutput.Default.Dispose();
        }

        [Test]
        public void server_can_be_open()
        {
            const string host = "127.0.0.1";
            const int port = 43712;

            using( var server = new Server.Server( host, port ) )
            {
                Action open = () => server.Open();
                open.ShouldNotThrow();
            }
        }

        [Test]
        public void simple_test()
        {
            true.ShouldBeEquivalentTo( 1 );
        }
    }
}