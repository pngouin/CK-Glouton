using CK.Core;
using CK.Glouton.Model.Server;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using CK.Monitoring;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using CK.Glouton.Model.Server.Handlers;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class AlertHandlerTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
        }

        [Test]
        public void alert_handler_do_not_crash()
        {
            Action openServer = () =>
            {
                using( var server = TestHelper.DefaultMockServer() )
                {
                    server.Should().NotBeNull();
                    server.Open( new HandlersManagerConfiguration { GloutonHandlers = { new AlertHandlerConfiguration { Alerts = null } } } );
                }
            };

            openServer.ShouldNotThrow();
        }

        [Test]
        public void sender_can_send_alerts()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                var sender = new TestAlertSender();

                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers =
                    {
                        new AlertHandlerConfiguration
                        {
                            Alerts = new List<(Func<ILogEntry, bool> condition, IList<IAlertSender> senders)>
                            {
                                ( log => (log.LogLevel & LogLevel.Fatal) == LogLevel.Fatal, new List<IAlertSender> { sender } ),
                                ( log => log.Text?.Contains( "Send" ) ?? false, new List<IAlertSender> { sender } )
                            }
                        }
                    }
                } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeFalse();

                    activityMonitor.Fatal( "Fatal Error n°42" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeTrue();
                    sender.Triggered = false;

                    activityMonitor.Info( "aze Send rty" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeTrue();
                }
            }
        }
    }

    internal class TestAlertSender : IAlertSender
    {
        public bool Triggered { get; set; }

        public void Send( ILogEntry logEntry )
        {
            Triggered = true;
        }
    }
}