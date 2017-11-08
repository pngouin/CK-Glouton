using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using CK.Core;
using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using FluentAssertions;
using NUnit.Framework;
using CK.Glouton.Lucene;

namespace CK.Glouton.Tests.NetFramework
{
    [TestFixture]
    public class LuceneTests 
    {
        [SetUp]
        public void SetUp()
        {
            GrandOuputServerHelper.SetupServer();
        }

        [TearDown]
        public void TearDown()
        {
            GrandOuputServerHelper.TearDown();
        }

        [Test]
        public void log_can_be_indexed ()
        {
            var server = TestHelper.DefaultServer();
            var m = new ActivityMonitor();
            m.MinimalFilter = LogFilter.Debug;
            server.Open();
            m.Info("Hello world");
            m.Error("CriticalError");
            server.Dispose();
        }
    }
}
