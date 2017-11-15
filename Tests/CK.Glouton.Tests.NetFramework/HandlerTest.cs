using CK.Core;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace CK.Glouton.Tests.NetFramework
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
            var m = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
            GrandOutputHelper.GrandOutputServer.EnsureGrandOutputClient( m );

            using( var server = TestHelper.DefaultServer() )
            {
                server.Open();

                var guid = Guid.NewGuid();

                m.Info( guid.ToString );

                var response = server.GetLogEntry( guid.ToString() );
                response.Should().Be( guid.ToString() );
            }
        }
    }
}
