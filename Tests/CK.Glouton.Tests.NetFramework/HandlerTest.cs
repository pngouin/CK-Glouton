using CK.Core;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class HandlerTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
        }

        [Test]
        public void handler_can_send_some_log()
        {
            using( var server = TestHelper.DefaultMockServer() )
            {
                server.Open();
                server.ListLog.Clear();

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                    var guid = Guid.NewGuid();
                    clientActivityMonitor.Info( guid.ToString );

                    Thread.Sleep( 500 );

                    var response = server.GetLogEntry( guid.ToString() );
                    response.Text.Should().Be( guid.ToString() );

                    serverActivityMonitor.CloseGroup();
                    clientActivityMonitor.CloseGroup();
                }
            }
        }

        [Test]
        public void handler_sends_multiple_logs()
        {
            using( var server = TestHelper.DefaultMockServer() )
            {
                server.Open();
                server.ListLog.Clear();

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                    const string initialMessage = "Hello";
                    const string errorMessage = "No, an error...";
                    const string debugMessage = "This is the error";
                    const string finalMessage = "It's only a goodbye";

                    clientActivityMonitor.Info( initialMessage );
                    clientActivityMonitor.Error( errorMessage );
                    clientActivityMonitor.Debug( debugMessage );
                    clientActivityMonitor.Info( finalMessage );

                    Thread.Sleep( 500 );

                    var initialEntry = server.GetLogEntry( initialMessage );
                    var errorEntry = server.GetLogEntry( errorMessage );
                    var debugEntry = server.GetLogEntry( debugMessage );
                    var finalEntry = server.GetLogEntry( finalMessage );

                    initialEntry.Text.Should().Be( initialMessage );
                    ( initialEntry.LogLevel & LogLevel.Info ).Should().Be( LogLevel.Info );

                    errorEntry.Text.Should().Be( errorMessage );
                    ( errorEntry.LogLevel & LogLevel.Error ).Should().Be( LogLevel.Error );

                    debugEntry.Text.Should().Be( debugMessage );
                    ( debugEntry.LogLevel & LogLevel.Debug ).Should().Be( LogLevel.Debug );

                    finalEntry.Text.Should().Be( finalMessage );
                    ( finalEntry.LogLevel & LogLevel.Info ).Should().Be( LogLevel.Info );

                    serverActivityMonitor.CloseGroup();
                    clientActivityMonitor.CloseGroup();
                }
            }
        }
    }
}