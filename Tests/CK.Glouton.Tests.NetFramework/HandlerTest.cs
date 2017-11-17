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

                using( var grandOutputServer = GrandOutputHelper.GetGrandOutputServer() )
                using( var grandOutputClient = GrandOutputHelper.GetGrandOutputClient() )
                {
                    var serverActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputServer.EnsureGrandOutputClient( serverActivityMonitor );

                    var clientActivityMonitor = new ActivityMonitor { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( clientActivityMonitor );

                    var guid = Guid.NewGuid();
                    clientActivityMonitor.Info( guid.ToString );

                    Thread.Sleep( 500 );

                    var response = server.GetLogEntry( guid.ToString() );
                    response.Text.Should().Be( guid.ToString() );

                    serverActivityMonitor.CloseGroup();
                    clientActivityMonitor.CloseGroup();

                }
            }
        }
    }
}
