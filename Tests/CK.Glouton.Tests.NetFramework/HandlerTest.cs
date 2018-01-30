using CK.Core;
using CK.Glouton.Handler.Tcp;
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

        //[Test]
        //public void handler_handles_multiple_clients()
        //{
        //    using( var server = TestHelper.DefaultMockServer() )
        //    {
        //        server.Open();

        //        using( var grandOutputServer = GrandOutputHelper.GetNewGrandOutputServer() )
        //        using( var grandOutputClient1 = GrandOutputHelper.GetNewGrandOutputClient() )
        //        {
        //            Thread.Sleep( TestHelper.DefaultSleepTime );
        //            using( var grandOutputClient2 = GrandOutputHelper.GetNewGrandOutputClient() )
        //            {
        //                Thread.Sleep( TestHelper.DefaultSleepTime );
        //                using( var grandOutputClient3 = GrandOutputHelper.GetNewGrandOutputClient() )
        //                {
        //                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
        //                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

        //                    Thread.Sleep( TestHelper.DefaultSleepTime );
        //                    var clients = new List<ActivityMonitor>();
        //                    var guids = new List<string>();

        //                    for( int i = 0 ; i < 3 ; i++ )
        //                    {
        //                        clients.Add( new ActivityMonitor { MinimalFilter = LogFilter.Debug } );

        //                        guids.Add( Guid.NewGuid().ToString() );
        //                    }

        //                    grandOutputClient1.EnsureGrandOutputClient( clients[ 0 ] );

        //                    Thread.Sleep( TestHelper.DefaultSleepTime );
        //                    grandOutputClient2.EnsureGrandOutputClient( clients[ 1 ] );

        //                    Thread.Sleep( TestHelper.DefaultSleepTime );
        //                    grandOutputClient3.EnsureGrandOutputClient( clients[ 2 ] );

        //                    for( int i = 0 ; i < clients.Count ; i++ )
        //                    {
        //                        clients[ i ].Info( guids[ i ] );
        //                        clients[ i ].CloseGroup();
        //                    }

        //                    Thread.Sleep( TestHelper.DefaultSleepTime );
        //                    var logs = server.CloseAndGetLogs();

        //                    for( int i = 0 ; i < guids.Count ; i++ )
        //                    {
        //                        logs.Any( l => l.Text == guids[ i ] ).Should().BeTrue();
        //                    }

        //                    serverActivityMonitor.CloseGroup();
        //                }
        //            }
        //        }
        //    }
        //}

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

        [Test]
        public void ApplyConfiguration()
        {
            var activityMonitor = new ActivityMonitor();
            var tcpHandlerConfiguration = new TcpHandlerConfiguration();
            var tcpHandler = new TcpHandler( tcpHandlerConfiguration );

            // ApplyConfiguration should always return false for now. Need to think about its implication before implementing.
            tcpHandler.ApplyConfiguration( activityMonitor, tcpHandlerConfiguration ).Should().BeFalse();
        }
    }
}