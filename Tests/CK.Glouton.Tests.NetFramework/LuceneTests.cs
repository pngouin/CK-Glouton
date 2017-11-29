using CK.Core;
using CK.Glouton.Lucene;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class LuceneTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
            var logDir = Assembly.GetExecutingAssembly().FullName.Split( ',' )[ 0 ];
            Array.ForEach( Directory.GetFiles( LuceneConstant.GetPath( logDir ) ), File.Delete );
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
                Thread.Sleep( TestHelper.DefaultSleepTime );
                g.Dispose();
            }
            var logDirectory = Assembly.GetExecutingAssembly().FullName.Split( ',' )[ 0 ];

            var searcher = new LuceneSearcher( LuceneConstant.GetPath( logDirectory ), new[] { "LogLevel", "Text" } );
            var topDocument = searcher.Search( "Hello world" );
            topDocument.Should().NotBeNull();
            topDocument = searcher.Search( "CriticalError" );
            topDocument.Should().NotBeNull();
        }
    }
}
