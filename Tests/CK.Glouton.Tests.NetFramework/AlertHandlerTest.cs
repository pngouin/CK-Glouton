using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using CK.Glouton.Server.Senders;
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
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers =
                    {
                        new AlertHandlerConfiguration { Alerts = new List<IAlertExpressionModel>
                        {
                            new AlertExpressionModelMock
                            (
                                new [] { new [] { "LogLevel", "In", "Fatal" } },
                                new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = "debug" } }
                            ),
                            new AlertExpressionModelMock
                            (
                                new [] { new [] { "Text", "Contains", "Send" } },
                                new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = "debug" } }
                            )
                        } }
                    }
                } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    // Assert false

                    activityMonitor.Fatal( "Fatal Error n°42" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    // Assert true

                    // Reset

                    activityMonitor.Info( "aze Send rty" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    // Assert true
                }
            }
        }

        [Test]
        public void can_add_alert()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration { GloutonHandlers = { new AlertHandlerConfiguration() } } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    // Assert false

                    server.ApplyConfiguration( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration { Alerts = new List<IAlertExpressionModel>
                            {
                                new AlertExpressionModelMock
                                (
                                    new [] { new [] { "Text", "EqualsTo", "Hello world" } },
                                    new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = "debug" } }
                                ),
                            } }
                        }
                    } );
                    Thread.Sleep( TestHelper.DefaultSleepTime );

                    activityMonitor.Info( "Hello world" );
                    Thread.Sleep( TestHelper.DefaultSleepTime );
                    // Assert true
                }
            }
        }
    }
}