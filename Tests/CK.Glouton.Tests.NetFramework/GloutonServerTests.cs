using FluentAssertions;
using NUnit.Framework;
using System;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class GloutonServerTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
        }

        [Test]
        public void server_can_be_open()
        {
            Action openServer = () =>
            {
                using( var server = TestHelper.DefaultMockServer() )
                {
                    server.Should().NotBeNull();
                    server.Open();
                }
            };

            openServer.ShouldNotThrow();
        }
    }
}