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

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                    var guid = Guid.NewGuid().ToString();
                    clientActivityMonitor.Info( guid );

                    Thread.Sleep( TestHelper.DefaultSleepTime );

                    server.GetLogEntry( guid ).Validate( guid ).Should().BeTrue();

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

                    Thread.Sleep( TestHelper.DefaultSleepTime * 2 );

                    server.GetLogEntry( initialMessage ).Validate( initialMessage, LogLevel.Info ).Should().BeTrue();
                    server.GetLogEntry( debugMessage ).Validate( debugMessage, LogLevel.Debug ).Should().BeTrue();
                    server.GetLogEntry( traceMessage ).Validate( traceMessage, LogLevel.Trace ).Should().BeTrue();
                    server.GetLogEntry( warnMessage ).Validate( warnMessage, LogLevel.Warn ).Should().BeTrue();
                    server.GetLogEntry( errorMessage ).Validate( errorMessage, LogLevel.Error ).Should().BeTrue();
                    server.GetLogEntry( fatalMessage ).Validate( fatalMessage, LogLevel.Fatal ).Should().BeTrue();
                    server.GetLogEntry( finalMessage ).Validate( finalMessage, LogLevel.Info ).Should().BeTrue();
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
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    using( var grandOutputClient2 = GrandOutputHelper.GetNewGrandOutputClient() )
                    {
                        Thread.Sleep( TestHelper.DefaultSleepTime );
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

                            var guid1 = Guid.NewGuid().ToString();
                            var guid2 = Guid.NewGuid().ToString();
                            var guid3 = Guid.NewGuid().ToString();

                            clientActivityMonitor1.Info( guid1 );
                            clientActivityMonitor2.Info( guid2 );
                            clientActivityMonitor3.Info( guid3 );

                            Thread.Sleep( TestHelper.DefaultSleepTime * 4 );

                            server.GetLogEntry( guid1 ).Validate( guid1 ).Should().BeTrue();
                            server.GetLogEntry( guid2 ).Validate( guid2 ).Should().BeTrue();
                            server.GetLogEntry( guid3 ).Validate( guid3 ).Should().BeTrue();

                            serverActivityMonitor.CloseGroup();
                            clientActivityMonitor1.CloseGroup();
                            clientActivityMonitor2.CloseGroup();
                            clientActivityMonitor3.CloseGroup();
                        }
                    }
                }
            }
        }

        [Test]
        public void close_and_reopen_server()
        {
            using( var server = TestHelper.DefaultMockServer() )
            {
                server.Open();

                using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                    {
                        var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                        grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                        var guid = Guid.NewGuid().ToString();

                        clientActivityMonitor.Info( guid );
                        Thread.Sleep( TestHelper.DefaultSleepTime );
                        server.GetLogEntry( guid ).Validate( guid ).Should().BeTrue();

                        serverActivityMonitor.Info( "Closing the server" );
                        server.Close();

                        server.ListLog.Clear();
                        clientActivityMonitor.Info( guid );
                        Thread.Sleep( TestHelper.DefaultSleepTime );
                        Action action = () => server.GetLogEntry( guid );
                        action.ShouldThrow<InvalidOperationException>();
                    }

                    serverActivityMonitor.Info( "Reopening server" );
                    server.Open();

                    using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                    {
                        var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                        grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                        var guid = Guid.NewGuid().ToString();

                        server.ListLog.Clear();
                        clientActivityMonitor.Info( guid );
                        Thread.Sleep( TestHelper.DefaultSleepTime );
                        server.GetLogEntry( guid ).Validate( guid ).Should().BeTrue();
                    }
                }
            }
        }
    }
}