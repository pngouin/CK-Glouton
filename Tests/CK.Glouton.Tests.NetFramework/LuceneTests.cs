using System;
using System.IO;
using System.Reflection;
using System.Threading;
using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using NUnit.Framework;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class LuceneTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.Setup();
            var logDir = Assembly.GetExecutingAssembly().FullName.Split( ',' )[0];
            var path = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "Glouton", "Logs", logDir );
            Array.ForEach( Directory.GetFiles( path ), File.Delete );
        }

        [Test]
        public void log_can_be_indexed_and_searched()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers = { new LuceneGloutonHandlerConfiguration() }
                } );
                var g = GrandOutputHelper.GetNewGrandOutputClient();

                var m = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                g.EnsureGrandOutputClient( m );

                m.Info( "Hello world" );
                m.Error( "CriticalError" );
                Thread.Sleep( TestHelper.DefaultSleepTime );
                g.Dispose();
            }

            var configuration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = TestHelper.GetTestLogDirectory(),
                Directory = ""
            };

            var searcher = new LuceneSearcher( configuration, new[] { "LogLevel", "Text" } );
            var topDocument = searcher.Search( "Hello world" );
            topDocument.Should().NotBeNull();
            topDocument = searcher.Search( "CriticalError" );
            topDocument.Should().NotBeNull();
        }
    }
}
