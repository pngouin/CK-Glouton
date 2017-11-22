using System;
using System.Threading;
using CK.Core;
using FluentAssertions;
using NUnit.Framework;

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

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                    var guid = Guid.NewGuid();
                    clientActivityMonitor.Info( guid.ToString );

                    Thread.Sleep( 125 );

                    server.GetLogEntry( guid.ToString() ).ValidateLogEntry( guid.ToString() ).Should().BeTrue();

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

                    Thread.Sleep( 125 );

                    server.GetLogEntry( initialMessage ).ValidateLogEntry( initialMessage, LogLevel.Info ).Should().BeTrue();
                    server.GetLogEntry( debugMessage ).ValidateLogEntry( debugMessage, LogLevel.Debug ).Should().BeTrue();
                    server.GetLogEntry( traceMessage ).ValidateLogEntry( traceMessage, LogLevel.Trace ).Should().BeTrue();
                    server.GetLogEntry( warnMessage ).ValidateLogEntry( warnMessage, LogLevel.Warn ).Should().BeTrue();
                    server.GetLogEntry( errorMessage ).ValidateLogEntry( errorMessage, LogLevel.Error ).Should().BeTrue();
                    server.GetLogEntry( fatalMessage ).ValidateLogEntry( fatalMessage, LogLevel.Fatal ).Should().BeTrue();
                    server.GetLogEntry( finalMessage ).ValidateLogEntry( finalMessage, LogLevel.Info ).Should().BeTrue();
                }
            }
        }

        [Test]
        public void handler_handles_multiple_clients()
        {
            using( var server = TestHelper.DefaultMockServer() )
            {
                server.Open();

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                using( var grandOutputClient1 = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    Thread.Sleep( 125 );
                    using( var grandOutputClient2 = GrandOutputHelper.GetNewGrandOutputClient() )
                    {
                        Thread.Sleep( 125 );
                        using( var grandOutputClient3 = GrandOutputHelper.GetNewGrandOutputClient() )
                        {
                            var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                            grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                            var clientActivityMonitor1 = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                            grandOutputClient1.EnsureGrandOutputClient( clientActivityMonitor1 );

                            var clientActivityMonitor2 = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                            grandOutputClient2.EnsureGrandOutputClient( clientActivityMonitor2 );

                            var clientActivityMonitor3 = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                            grandOutputClient3.EnsureGrandOutputClient( clientActivityMonitor3 );

                            var guid1 = Guid.NewGuid();
                            var guid2 = Guid.NewGuid();
                            var guid3 = Guid.NewGuid();

                            clientActivityMonitor1.Info( guid1.ToString );
                            clientActivityMonitor2.Info( guid2.ToString );
                            clientActivityMonitor3.Info( guid3.ToString );

                            Thread.Sleep( 125 );

                            server.GetLogEntry( guid1.ToString() ).ValidateLogEntry( guid1.ToString() ).Should().BeTrue();
                            server.GetLogEntry( guid2.ToString() ).ValidateLogEntry( guid2.ToString() ).Should().BeTrue();
                            server.GetLogEntry( guid3.ToString() ).ValidateLogEntry( guid3.ToString() ).Should().BeTrue();

                            serverActivityMonitor.CloseGroup();
                            clientActivityMonitor1.CloseGroup();
                            clientActivityMonitor2.CloseGroup();
                            clientActivityMonitor3.CloseGroup();
                        }
                    }
                }
            }
        }
    }
}