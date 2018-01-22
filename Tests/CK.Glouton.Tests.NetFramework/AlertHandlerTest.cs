using CK.Core;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IExpressionModel = CK.Glouton.Model.Server.Handlers.IExpressionModel;

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
                            Alerts = new List<IAlertExpressionModel>
                            {
                                new TestAlertExpressionModel( new [] { new [] { "LogLevel", "In", "Fatal" } } ),
                                new TestAlertExpressionModel( new [] { new [] { "Text", "Contains", "Send" } } )
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
                                Alerts = new List<IAlertExpressionModel>
                                {
                                    new TestAlertExpressionModel( new [] { new [] { "Text", "EqualsTo", "Hello world" } } ),
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

    internal class TestAlertExpressionModel : IAlertExpressionModel
    {
        public IExpressionModel[] Expressions { get; set; }
        public IAlertSenderConfiguration[] Senders { get; set; }

        public TestAlertExpressionModel( IEnumerable<string[]> expressions )
        {
            Expressions = expressions
                    .Select( expression => new ExpressionModel { Field = expression[ 0 ], Operation = expression[ 1 ], Body = expression[ 2 ] } )
                    .ToArray();
        }

        internal class ExpressionModel : IExpressionModel
        {
            public string Field { get; set; }
            public string Operation { get; set; }
            public string Body { get; set; }
        }
    }

    internal class TestAlertSender : IAlertSender
    {
        public bool Triggered { get; private set; }

        public TestAlertSender()
        {
            Triggered = false;
        }

        public string SenderType { get; set; } = "Test";

        public bool Match( IAlertSenderConfiguration configuration )
        {
            return configuration.SenderType == "Test";
        }

        public void Send( AlertEntry logEntry )
        {
            Triggered = true;
        }

        public void Reset()
        {
            Triggered = false;
        }
    }
}