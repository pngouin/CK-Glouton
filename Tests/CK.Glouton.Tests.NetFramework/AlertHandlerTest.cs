using CK.Core;
using CK.Glouton.Model.Server;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using CK.Monitoring;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
            var sender = new TestAlertSender();

            Action openServer = () =>
            {
                using( var server = TestHelper.DefaultMockServer() )
                {
                    server.Should().NotBeNull();
                    server.Open( new HandlersManagerConfiguration
                    {
                        GloutonHandlers =
                        {
                            new AlertHandlerConfiguration
                            {
                                Alerts = new List<(Func<ILogEntry, bool> condition, IList<IAlertSender> sender)>
                                {
                                    ( log => log.LogLevel == LogLevel.Fatal, new List<IAlertSender> { sender } )
                                }
                            }
                        }
                    } );
                }
            };

            openServer.ShouldNotThrow();
        }
    }

    internal class TestAlertSender : IAlertSender
    {
        private readonly IActivityMonitor _activityMonitor;

        public TestAlertSender()
        {
            _activityMonitor = new ActivityMonitor();
        }

        public void Send( ILogEntry logEntry )
        {
            _activityMonitor.Info( "Sent alert." );
        }
    }
}