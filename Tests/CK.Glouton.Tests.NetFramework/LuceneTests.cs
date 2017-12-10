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
using System.Collections.Generic;
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

        public static AggregateException ThrowAggregateException(int numberOfException)
        {
            List<Exception> exceptions = new List<Exception>();
            for (int i = 0; i < numberOfException; i++)
            {
                try { throw new Exception(); }
                catch (Exception ex) { exceptions.Add(ex); }
            }

            return new AggregateException("Aggregate exceptions list", exceptions);
        }

        [Test]
        public void log_can_be_indexed_and_searched_with_full_text_search()
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

            LuceneSearcherManager searcherManager = new LuceneSearcherManager(LuceneSearcherConfiguration);
            var searcher = searcherManager.GetSearcher(LuceneSearcherConfiguration.Directory);

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                Fields = new[] { "LogLevel", "Text" },
                SearchMethod = SearchMethod.FullText,
                MaxResult = 10,
                AppName = new string[] { LuceneSearcherConfiguration.Directory },
                Query = "Text:\"Hello world\""
            };

            var result = searcher.Search(configuration);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            var log = result[0] as LineViewModel;
            log.Text.Should().Be("Hello world");
            log.LogLevel.Should().Contain("Info");

            configuration.Query = "Text:\"CriticalError\"";

            result = searcher.Search(configuration);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            log = result[0] as LineViewModel;
            log.Text.Should().Be("CriticalError");
            log.LogLevel.Should().Contain("Error");
        }

        [Test]
        public void log_can_be_indexed_and_searched_with_object_search()
        {
            using (var server = TestHelper.DefaultGloutonServer())
            {
                server.Open(new HandlersManagerConfiguration
                {
                    GloutonHandlers = { LuceneGloutonHandlerConfiguration }
                });

                using (var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient())
                {
                    var activityMonitor = new ActivityMonitor(false) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient(activityMonitor);

                    activityMonitor.Info("Hello world");
                    activityMonitor.Error("CriticalError");
                    using (activityMonitor.OpenFatal(new Exception("Fatal")))
                    {
                        activityMonitor.Info(new Exception());
                    }
                }
            }

            LuceneSearcherManager searcherManager = new LuceneSearcherManager(LuceneSearcherConfiguration);
            var searcher = searcherManager.GetSearcher(LuceneSearcherConfiguration.Directory);

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                Fields = new[] { "Text" },
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                AppName = new string[] { LuceneSearcherConfiguration.Directory },
                Query = "\"Hello world\""
            };

            var result = searcher.Search(configuration);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            var log = result[0] as LineViewModel;
            log.Text.Should().Be("Hello world");
            log.LogLevel.Should().Contain("Info");

            configuration.Query = "CriticalError";

            result = searcher.Search(configuration);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            log = result[0] as LineViewModel;
            log.Text.Should().Be("CriticalError");
            log.LogLevel.Should().Contain("Error");
        }

        [Test]
        public void log_with_aggragatedexception_can_be_indexed_and_searched()
        {
            using (var server = TestHelper.DefaultGloutonServer())
            {
                server.Open(new HandlersManagerConfiguration
                {
                    GloutonHandlers = { LuceneGloutonHandlerConfiguration }
                });

                using (var grandOutputClient = GrandOutputHelper.GetNewGrandOutputClient())
                {
                    var activityMonitor = new ActivityMonitor(false) { MinimalFilter = LogFilter.Debug };
                    grandOutputClient.EnsureGrandOutputClient(activityMonitor);
                    activityMonitor.Fatal(ThrowAggregateException(3));
                }
            }

            LuceneSearcherManager searcherManager = new LuceneSearcherManager(LuceneSearcherConfiguration);
            var searcher = searcherManager.GetSearcher(LuceneSearcherConfiguration.Directory);

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                MaxResult = 10,
                AppName = new string[] { LuceneSearcherConfiguration.Directory }
            };
            configuration.SearchAll(LuceneWantAll.Exception);

            var result = searcher.Search(configuration);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].LogType.Should().Be(ELogType.Line);

            var log = result[0] as LineViewModel;
            log.Exception.Should().NotBeNull();
            log.LogLevel.Should().Contain("Fatal");
            log.Exception.Message.Should().Be("Aggregate exceptions list");
            log.Exception.AggregatedExceptions.Should().NotBeNull();
            log.Exception.AggregatedExceptions.Count.Should().Be(3);

            foreach( var exception in log.Exception.AggregatedExceptions)
            {
                exception.Message.Should().NotBeNullOrEmpty();
                exception.StackTrace.Should().NotBeNullOrEmpty();
            }
        }
    }
}
