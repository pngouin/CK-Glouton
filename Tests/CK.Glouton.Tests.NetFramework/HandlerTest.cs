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
                    const string debugMessage = "This is the error";
                    const string traceMessage = "Weird/trace/message";
                    const string warnMessage = "You forgot something in the oven";
                    const string errorMessage = "No, an error...";
                    const string fatalMessage = "A fatal";
                    const string finalMessage = "It's only a goodbye";

                    clientActivityMonitor.Info( initialMessage );
                    clientActivityMonitor.Debug( debugMessage );
                    clientActivityMonitor.Trace( traceMessage );
                    clientActivityMonitor.Warn( warnMessage );
                    clientActivityMonitor.Error( errorMessage );
                    clientActivityMonitor.Fatal( fatalMessage );
                    clientActivityMonitor.Info( finalMessage );

                    Thread.Sleep( 500 );

                    var initialEntry = server.GetLogEntry( initialMessage );
                    var debugEntry = server.GetLogEntry( debugMessage );
                    var traceEntry = server.GetLogEntry( traceMessage );
                    var warnEntry = server.GetLogEntry( warnMessage );
                    var errorEntry = server.GetLogEntry( errorMessage );
                    var fatalEntry = server.GetLogEntry( fatalMessage );
                    var finalEntry = server.GetLogEntry( finalMessage );

                    initialEntry.Text.Should().Be( initialMessage );
                    ( initialEntry.LogLevel & LogLevel.Info ).Should().Be( LogLevel.Info );

                    debugEntry.Text.Should().Be( debugMessage );
                    ( debugEntry.LogLevel & LogLevel.Debug ).Should().Be( LogLevel.Debug );

                    traceEntry.Text.Should().Be( traceMessage );
                    ( traceEntry.LogLevel & LogLevel.Trace ).Should().Be( LogLevel.Trace );

                    warnEntry.Text.Should().Be( warnMessage );
                    ( warnEntry.LogLevel & LogLevel.Warn ).Should().Be( LogLevel.Warn );

                    errorEntry.Text.Should().Be( errorMessage );
                    ( errorEntry.LogLevel & LogLevel.Error ).Should().Be( LogLevel.Error );

                    fatalEntry.Text.Should().Be( fatalMessage );
                    ( fatalEntry.LogLevel & LogLevel.Fatal ).Should().Be( LogLevel.Fatal );

                    finalEntry.Text.Should().Be( finalMessage );
                    ( finalEntry.LogLevel & LogLevel.Info ).Should().Be( LogLevel.Info );

                    serverActivityMonitor.CloseGroup();
                    clientActivityMonitor.CloseGroup();
                }
            }
        }
    }
}