﻿using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
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
            var path = TestHelper.GetTestLogDirectory();
            Array.ForEach( Directory.GetFiles( path ), f =>
               {
                   if( File.Exists( f ) )
                       File.Delete( f );
               } );
        }

        private const int LuceneMaxSearch = 10;
        private static readonly string LucenePath = Path.Combine( TestHelper.GetTestLogDirectory(), "Lucene" );

        private static readonly LuceneGloutonHandlerConfiguration LuceneGloutonHandlerConfiguration = new LuceneGloutonHandlerConfiguration
        {
            MaxSearch = LuceneMaxSearch,
            Path = LucenePath,
            Directory = ""
        };

        private static readonly LuceneConfiguration LuceneSearcherConfiguration = new LuceneConfiguration
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
                }
            }

            Thread.Sleep( TestHelper.DefaultSleepTime * 4 );

            var searcher = new LuceneSearcher( LuceneSearcherConfiguration, new[] { "LogLevel", "Text" } );
            var topDocument = searcher.Search( "Hello world" );
            topDocument.Should().NotBeNull();
            topDocument = searcher.Search( "CriticalError" );
            topDocument.Should().NotBeNull();
        }
    }
}
