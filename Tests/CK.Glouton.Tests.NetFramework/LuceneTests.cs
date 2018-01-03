using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using FluentAssertions;
using Lucene.Net.Index;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

#if NET461
        [TestFixtureSetUp]
#else
        [OneTimeSetUp]
#endif
        public void ConstructIndex()
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
                    activityMonitor.Fatal( ThrowAggregateException( 3 ) );

                }
            }
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

        private static readonly LuceneConfiguration LuceneSearcherConfiguration = new LuceneConfiguration
        {
            MaxSearch = LuceneMaxSearch,
            Path = LucenePath,
            Directory = Assembly.GetExecutingAssembly().GetName().Name
        };

        public static AggregateException ThrowAggregateException( int numberOfException )
        {
            List<Exception> exceptions = new List<Exception>();
            for( int i = 0 ; i < numberOfException ; i++ )
            {
                try
                { throw new Exception(); }
                catch( Exception ex ) { exceptions.Add( ex ); }
            }

            return new AggregateException( "Aggregate exceptions list", exceptions );
        }

        [Test]
        public void log_can_be_indexed_and_searched_with_full_text_search()
        {


            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            var searcher = searcherManager.GetSearcher( LuceneSearcherConfiguration.Directory );

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                Fields = new[] { "LogLevel", "Text" },
                SearchMethod = SearchMethod.FullText,
                MaxResult = 10,
                Query = "Text:\"Hello world\""
            };

            var result = searcher.Search( configuration );
            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            var log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "Hello world" );
            log.LogLevel.Should().Contain( "Info" );

            configuration.Query = "Text:\"CriticalError\"";

            result = searcher.Search( configuration );
            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "CriticalError" );
            log.LogLevel.Should().Contain( "Error" );
        }

        [Test]
        public void log_can_be_indexed_and_searched_with_object_search()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            var searcher = searcherManager.GetSearcher( LuceneSearcherConfiguration.Directory );

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                Fields = new[] { "Text" },
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                Query = "\"Hello world\""
            };

            //
            // Search an all document with `Text` field equal to "Hello world"
            //
            var result = searcher.Search( configuration );
            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            var log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "Hello world" );
            log.LogLevel.Should().Contain( "Info" );
            log.Tags.Should().BeOfType<string>();
            log.SourceFileName.Should().BeOfType<string>();
            log.LineNumber.Should().BeOfType<string>();
            log.LogLevel.Should().BeOfType<string>();
            log.MonitorId.Should().BeOfType<string>();
            log.GroupDepth.Should().BeOfType( typeof( int ) );
            log.PreviousEntryType.Should().BeOfType<string>();
            log.PreviousLogTime.Should().BeOfType<string>();
            log.AppName.Should().BeOfType<string>();
            log.LogTime.Should().BeOfType<string>();
            log.Exception.Should().BeNull();

            //
            // Search an all document with `Text` field equal to "CriticalError"
            //
            configuration.Query = "CriticalError";
            result = searcher.Search( configuration );

            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "CriticalError" );
            log.LogLevel.Should().Contain( "Error" );



            configuration.SearchAll( LuceneWantAll.Log );
            result = searcher.Search( configuration );
            result.Count.Should().Be( 8 ); // TODO: If add log to the index change the number or get an alternative technique.

            //
            // Search all document with LogLevel between 0002-01-01 to 9999-01-01
            //
            configuration = new LuceneSearcherConfiguration
            {
                Fields = new string[ 0 ],
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                DateStart = new DateTime( 2, 01, 01 ),
                DateEnd = new DateTime( 9999, 01, 01 )
            };
            result = searcher.Search( configuration );
            result.Count.Should().Be( 8 );

            //
            // Search all document with LogLevel between 0002-01-01 to 0003-01-01
            //
            configuration = new LuceneSearcherConfiguration
            {
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                DateStart = new DateTime( 2, 01, 01 ),
                DateEnd = new DateTime( 3, 01, 01 )
            };
            result = searcher.Search( configuration );
            result.Count.Should().Be( 0 );


            //
            // Search all MonitorId in all appname contain in the searcher
            //
            var monitorId = searcher.GetAllMonitorId();
            monitorId.Count.Should().Be( 2 );

            //
            // Search with false MonitorId
            //
            configuration = new LuceneSearcherConfiguration
            {
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                MonitorId = Guid.NewGuid().ToString()
            };
            result = searcher.Search( configuration );
            result.Count.Should().Be( 0 );

            //
            // Search all fatal log
            //
            configuration = new LuceneSearcherConfiguration
            {
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                LogLevel = new string[] { "Fatal" }
            };
            result = searcher.Search( configuration );
            result.Count.Should().Be( 1 );

            //
            // Search all log with GroupDepth 
            //
            configuration = new LuceneSearcherConfiguration
            {
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                GroupDepth = 1
            };
            result = searcher.Search(configuration);
            result.Count.Should().Be(2);

            //
            // Search all document with a LogLevel and a monitorId
            //
            configuration = new LuceneSearcherConfiguration
            {
                SearchMethod = SearchMethod.WithConfigurationObject,
                MaxResult = 10,
                Fields = new string[] { LogField.MONITOR_ID }
            };
            result = searcher.Search( configuration );
            result.Count.Should().Be( 8 );
        }

        [Test]
        public void get_searcher_with_bad_appname()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            var s = searcherManager.GetSearcher( new[] { Guid.NewGuid().ToString() } );
            s.Should().BeNull();
        }

        [Test]
        public void luceneSearcherManager_return_log_order_by_date()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager(LuceneSearcherConfiguration);
            var searcher = searcherManager.GetSearcher(LuceneSearcherConfiguration.Directory);

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration();
            configuration.SearchAll(LuceneWantAll.Log);
            var result = searcher.Search(configuration);

            result.SequenceEqual(result.OrderByDescending(l => l.LogTime)).Should().BeTrue();
        }

        [Test]
        public void luceneSearcherManager_return_good_appName()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            string directoryPath = LuceneSearcherConfiguration.Path + "\\" + Guid.NewGuid().ToString();

            Directory.CreateDirectory( directoryPath );

            var appName = searcherManager.AppName;
            appName.Count.Should().NotBe(Directory.GetDirectories(LuceneSearcherConfiguration.Path).Length);
            appName.Contains(LuceneSearcherConfiguration.Directory).Should().BeTrue();

            Directory.Delete( directoryPath );
        }

        [Test]
        public void bad_configuration_should_throw_exception()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            var searcher = searcherManager.GetSearcher( LuceneSearcherConfiguration.Directory );

            Action action = () => searcher.Search( null );
            action.ShouldThrow<ArgumentNullException>();

            action = () => searcher.Search( new LuceneSearcherConfiguration() );
            action.ShouldThrow<ArgumentException>();

        }

        [Test]
        public void log_with_aggregated_exception_can_be_indexed_and_searched()
        {
            LuceneSearcherManager searcherManager = new LuceneSearcherManager( LuceneSearcherConfiguration );
            var searcher = searcherManager.GetSearcher( LuceneSearcherConfiguration.Directory );

            LuceneSearcherConfiguration configuration = new LuceneSearcherConfiguration
            {
                MaxResult = 10,
            };
            configuration.SearchAll( LuceneWantAll.Exception );

            var result = searcher.Search( configuration );
            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            var log = result[ 0 ] as LineViewModel;
            log.Exception.Should().NotBeNull();
            log.LogLevel.Should().Contain( "Fatal" );
            log.Exception.Message.Should().Contain( "Aggregate exceptions list" );
            log.Exception.AggregatedExceptions.Should().NotBeNull();
            log.Exception.AggregatedExceptions.Count.Should().Be( 3 );

            foreach( var exception in log.Exception.AggregatedExceptions )
            {
                exception.Message.Should().NotBeNullOrEmpty();
                exception.StackTrace.Should().NotBeNullOrEmpty();
            }
        }
    }
}
