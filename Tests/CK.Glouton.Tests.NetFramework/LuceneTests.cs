using CK.Core;
using NUnit.Framework;
using FluentAssertions;
using System.Threading;
using CK.Glouton.Lucene;
using System.Reflection;
using System;
using System.IO;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class LuceneTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
            var logDir = Assembly.GetExecutingAssembly().FullName.Split(',')[0];
            Array.ForEach(Directory.GetFiles(LuceneConstant.GetPath(logDir)), File.Delete);
        }

        [Test]
        public void log_can_be_indexed_and_searched()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open();
                var g = GrandOutputHelper.GetNewGrandOutputClient();

                var m = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                g.EnsureGrandOutputClient( m );

                m.Info( "Hello world" );
                m.Error( "CriticalError" );
                Thread.Sleep(500);
                g.Dispose();
            }
            var logDir = Assembly.GetExecutingAssembly().FullName.Split(',')[0];

            LuceneSearcher searcher = new LuceneSearcher(LuceneConstant.GetPath(logDir), new[] { "LogLevel", "Text" });
            var topdoc = searcher.Search("Hello world");
            Assert.That(topdoc != null);
            topdoc = null;
            topdoc = searcher.Search("CriticalError");
            Assert.That(topdoc != null);
        }
    }
}
