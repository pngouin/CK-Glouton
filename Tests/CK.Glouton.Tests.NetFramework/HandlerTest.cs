using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CK.Core;
using FluentAssertions;

namespace CK.Glouton.Tests.NetFramework
{
    [TestFixture]
    public class HandlerTest
    {
        [SetUp]
        public void SetUp()
        {
            GrandOuputServerHelper.SetupServer();
            GrandOutputHandlerHelper.SetupHandler();
        }

        [TearDown]
        public void TearDown()
        {
            //GrandOuputServerHelper.TearDown();
            //GrandOutputHandlerHelper.TearDown();
        }

        [Test]
        public void handler_can_send_some_log()
        {
            var server = TestHelper.DefaultServer();
            var m = new ActivityMonitor();
            m.MinimalFilter = LogFilter.Debug;
            server.Open();

            var guid = Guid.NewGuid();

            m.Info(guid.ToString);
            var response = server.GetLogEntry(guid.ToString());

            response.Should().Be(guid.ToString());
            server.Dispose();
        }
    }
}
