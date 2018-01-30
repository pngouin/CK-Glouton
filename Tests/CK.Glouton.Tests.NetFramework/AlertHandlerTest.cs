using CK.Core;
using CK.Glouton.AlertSender.Sender;
using CK.Glouton.Common;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
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
                    server.Open( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration { DatabasePath = @"%localappdata%/Glouton/Alerts".GetPathWithSpecialFolders() }
                        }
                    } );
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
                        new AlertHandlerConfiguration {
                            DatabasePath = @"%localappdata%/Glouton/Alerts".GetPathWithSpecialFolders(),
                            Alerts = new List<IAlertExpressionModel>
                            {
                                new AlertExpressionModelMock
                                (
                                    new [] { new [] { "LogLevel", "In", "Fatal" } },
                                    new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = SingleHttpReceiver.DefaultUrl } }
                                ),
                                new AlertExpressionModelMock
                                (
                                    new [] { new [] { "Text", "Contains", "Send" } },
                                    new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = SingleHttpReceiver.DefaultUrl } }
                                )
                            }
                        }
                    }
                } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    using( var receiver = new SingleHttpReceiver( SingleHttpReceiver.DefaultUrl ) )
                    {

                        activityMonitor.Info( "Send Hello world" );
                        Thread.Sleep( TestHelper.DefaultSleepTime * 10 );
                        receiver.Alerted.Should().BeTrue();
                    }

                    using( var receiver = new SingleHttpReceiver( SingleHttpReceiver.DefaultUrl ) )
                    {
                        activityMonitor.Fatal( "Fatal Error n°42" );
                        Thread.Sleep( TestHelper.DefaultSleepTime * 10 );
                        receiver.Alerted.Should().BeTrue();
                    }
                }
            }
        }

        [Test]
        public void can_add_alert()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers =
                    {
                        new AlertHandlerConfiguration { DatabasePath = @"%localappdata%/Glouton/Alerts".GetPathWithSpecialFolders() }
                    }
                } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    using( var receiver = new SingleHttpReceiver( SingleHttpReceiver.DefaultUrl ) )
                    {
                        activityMonitor.Info( "Hello world" );
                        Thread.Sleep( TestHelper.DefaultSleepTime );
                        receiver.Alerted.Should().BeFalse();
                    }

                    server.ApplyConfiguration( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration {
                                DatabasePath = @"%localappdata%/Glouton/Alerts".GetPathWithSpecialFolders(),
                                Alerts = new List<IAlertExpressionModel>
                                {
                                    new AlertExpressionModelMock
                                    (
                                        new [] { new [] { "Text", "EqualTo", "Hello world" } },
                                        new IAlertSenderConfiguration[] { new HttpSenderConfiguration { Url = SingleHttpReceiver.DefaultUrl } }
                                    )
                                }
                            }
                        }
                    } );
                    Thread.Sleep( TestHelper.DefaultSleepTime );

                    using( var receiver = new SingleHttpReceiver( SingleHttpReceiver.DefaultUrl ) )
                    {
                        activityMonitor.Info( "Hello world" );
                        Thread.Sleep( TestHelper.DefaultSleepTime * 10 );
                        receiver.Alerted.Should().BeTrue();
                    }
                }
            }
        }
    }
}