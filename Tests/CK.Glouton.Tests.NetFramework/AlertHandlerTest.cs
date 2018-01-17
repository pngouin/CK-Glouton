using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

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
        public void can_send_alerts()
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
                            Alerts = new List<IAlertModel>
                            {
                                new TestAlertModel { Condition = log => (log.LogLevel & LogLevel.Fatal) == LogLevel.Fatal, Senders = new List<IAlertSender> { sender } },
                                new TestAlertModel { Condition = log => log.Text?.Contains( "Send" ) ?? false, Senders = new List<IAlertSender> { sender } }
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

                    sender.Reset();

                    activityMonitor.Info( "aze Send rty" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeTrue();
                }
            }
        }

        [Test]
        public void can_add_alert()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                var sender = new TestAlertSender();

                server.Open( new HandlersManagerConfiguration { GloutonHandlers = { new AlertHandlerConfiguration() } } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeFalse();

                    server.ApplyConfiguration( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration
                            {
                                Alerts = new List<IAlertModel>
                                {
                                    new TestAlertModel { Condition = log => log.Text?.Equals("Hello world") ?? false, Senders = new List<IAlertSender> { sender } }
                                }
                            }
                        }
                    } );
                    Thread.Sleep( TestHelper.DefaultSleepTime );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    sender.Triggered.Should().BeTrue();
                }
            }
        }
    }

    internal class TestAlertModel : IAlertModel
    {
        public Func<IAlertEntry, bool> Condition { get; set; }
        public IList<IAlertSender> Senders { get; set; }
    }

    internal class TestAlertSender : IAlertSender
    {
        public bool Triggered { get; private set; }

        public TestAlertSender()
        {
            Triggered = false;
        }

        public void Send( IAlertEntry logEntry )
        {
            Triggered = true;
        }

        public void Reset()
        {
            Triggered = false;
        }
    }
}