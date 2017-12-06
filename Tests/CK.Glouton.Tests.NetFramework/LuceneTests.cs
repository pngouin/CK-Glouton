using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
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
        }

        private const int LuceneMaxSearch = 10;
        private static readonly string LucenePath = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" );

        private static readonly LuceneGloutonHandlerConfiguration LuceneGloutonHandlerConfiguration = new LuceneGloutonHandlerConfiguration
        {
            MaxSearch = LuceneMaxSearch,
            Path = LucenePath,
            OpenMode = OpenMode.CREATE,
            Directory = ""
        };

        private  static readonly LuceneConfiguration LuceneSearcherConfiguration = new LuceneConfiguration
        {
            MaxSearch = LuceneMaxSearch,
            Path = LucenePath,
            Directory = Assembly.GetExecutingAssembly().GetName().Name
        };

        [Test]
        public void log_can_be_indexed_and_searched()
        {
            using( var server = TestHelper.DefaultGloutonServer() )
            {
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers = { LuceneGloutonHandlerConfiguration }
                } );

                using( var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient() )
                {
                    var activityMonitor = new ActivityMonitor( false ) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient( activityMonitor );

                    activityMonitor.Info( "Hello world" );
                    activityMonitor.Error( "CriticalError" );
                    using( activityMonitor.OpenFatal( new Exception( "Fatal" ) ) )
                    {
                        activityMonitor.Info( new Exception() );
                    }
                }
            }

            var searcher = new LuceneSearcher( LuceneSearcherConfiguration, new[] { "LogLevel", "Text" } );
            var result = searcher.Search("Text:\"Hello world\"");
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            var log = result[0] as LineViewModel;
            log.Text.Should().Be("Hello world");
            log.LogLevel.Should().Contain("Info");


            result = searcher.Search("Text:\"CriticalError\"");
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            log = result[0] as LineViewModel;
            log.Text.Should().Be("CriticalError");
            log.LogLevel.Should().Contain("Error");
        }
    }
}
