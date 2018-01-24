using System;
using System.Collections.Generic;
using System.Threading;
using CK.Core;
using CK.Glouton.AlertSender.Sender;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using NUnit.Framework;

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
                    server.Open( new HandlersManagerConfiguration { GloutonHandlers = { new AlertHandlerConfiguration() } } );
                }
            };

            openServer.ShouldNotThrow();
        }

        [Test]
        public void can_send_alerts()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers =
                    {
                        new AlertHandlerConfiguration { Alerts = new List<IAlertExpressionModel>
                        {
                            new AlertExpressionModelMock
                            (
                                new [] { new [] { "LogLevel", "In", "Fatal" } },
                                new IAlertSenderConfiguration[] { new HttpSenderSenderConfiguration { Url = HttpServerReceiver.DefaultUrl } }
                            ),
                            new AlertExpressionModelMock
                            (
                                new [] { new [] { "Text", "Contains", "Send" } },
                                new IAlertSenderConfiguration[] { new HttpSenderSenderConfiguration { Url = HttpServerReceiver.DefaultUrl } }
                            )
                        } }
                    }
                } );

                using( var httpServer = new HttpServerReceiver( HttpServerReceiver.DefaultUrl ) )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "send Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    httpServer.Alerted.Should().BeFalse();

                    activityMonitor.Fatal( "Fatal Error n°42" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    httpServer.Alerted.Should().BeTrue();
                }
            }
        }

        [Test]
        public void can_add_alert()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration { GloutonHandlers = { new AlertHandlerConfiguration() } } );

                using( var httpServer = new HttpServerReceiver( HttpServerReceiver.DefaultUrl ) )
                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    httpServer.Alerted.Should().BeFalse();

                    server.ApplyConfiguration( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration { Alerts = new List<IAlertExpressionModel>
                            {
                                new AlertExpressionModelMock
                                (
                                    new [] { new [] { "Text", "EqualTo", "Hello world" } },
                                    new IAlertSenderConfiguration[] { new HttpSenderSenderConfiguration { Url = HttpServerReceiver.DefaultUrl } }
                                )
                            } }
                        }
                    } );
                    Thread.Sleep( TestHelper.DefaultSleepTime );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime * 10 );
                    httpServer.Alerted.Should().BeTrue();
                }
            }
        }
    }
}