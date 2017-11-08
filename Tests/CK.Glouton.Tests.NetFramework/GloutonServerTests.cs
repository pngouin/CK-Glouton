using CK.Core;
using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Text;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class GloutonServerTests : NetFramework.TestsSetupTeardown
    {
        [Test]
        public void server_can_be_open()
        {
            Action openServer = () =>
            {
                var server = TestHelper.DefaultServer();
                server.Should().NotBeNull();
                server.Open();
                server.Dispose();
            };

            openServer.ShouldNotThrow();
        }
    }
}