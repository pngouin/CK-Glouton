using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using CK.Glouton.Model.Logs.Implementation;
using CK.Glouton.Service;
using FluentAssertions;
using Lucene.Net.Index;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace CK.Glouton.Tests
{
    [TestFixture]
    public class ServicesTests

    {

#if NET461
        [TestFixtureSetUp]
#else
        [OneTimeSetUp]
#endif
        public void ConstructIndex()
        {
            LuceneTestIndexBuilder.ConstructIndex();
        }

        [Test]
        public void log_can_be_search()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" ),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };
            var directory = Assembly.GetExecutingAssembly().GetName().Name;

            var searcherService = new LuceneSearcherService( luceneConfiguration );

            var result = searcherService.Search( "Text:\"Hello world\"", directory );

            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            var log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "Hello world" );
            log.LogLevel.Should().Contain( "Info" );

            result = searcherService.Search( "Text:\"CriticalError\"", directory );
            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "CriticalError" );
            log.LogLevel.Should().Contain( "Error" );
        }

        [Test]
        public void can_get_specific_log()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" ),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };

            var directory = Assembly.GetExecutingAssembly().GetName().Name;

            var searcherService = new LuceneSearcherService( luceneConfiguration );

            var result = searcherService.GetLogWithFilters( null, new DateTime( 2, 01, 01 ), new DateTime( 9999, 01, 01 ), new[] { "LogLevel", "Text" }, new[] { "Info" }, "Text:\"Hello world\"", new[] { directory }, 0 );

            result.Should().NotBeNull();
            result.Count.Should().Be( 1 );
            result[ 0 ].LogType.Should().Be( ELogType.Line );

            var log = result[ 0 ] as LineViewModel;
            log.Text.Should().Be( "Hello world" );
            log.LogLevel.Should().Contain( "Info" );
        }

        [Test]
        public void can_get_all_logs()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" ),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };

            var directory = Assembly.GetExecutingAssembly().GetName().Name;

            var searcherService = new LuceneSearcherService( luceneConfiguration );

            var result = searcherService.GetAll( new[] { directory } );
            result.Should().NotBeNull();
            result.Count.Should().Be( LuceneTestIndexBuilder.TotalLogCount );
        }

        [Test]
        public void can_get_all_appName()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" ),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };

            var searcherService = new LuceneSearcherService( luceneConfiguration );

            var result = searcherService.GetAppNameList();
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan( 0 );
            result.Should().Contain( Assembly.GetExecutingAssembly().GetName().Name );
        }

        [Test]
        public void can_get_monitor_id_list()
        {
            var luceneConfiguration = new LuceneConfiguration
            {
                MaxSearch = 10,
                Path = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" ),
                OpenMode = OpenMode.CREATE,
                Directory = ""
            };

            var searcherService = new LuceneSearcherService( luceneConfiguration );

            var result = searcherService.GetMonitorIdList();
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan( 0 );
        }
    }
}
