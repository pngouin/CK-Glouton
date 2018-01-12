using CK.Glouton.Model.Server;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
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
                                Alerts = new List<(Func<ReceivedData, bool> condition, IAlertSender sender)>
                                {
                                    (data => data.Data == null, null)
                                }
                            }
                        }
                    } );
                }
            };

            openServer.ShouldNotThrow();
        }
    }
}