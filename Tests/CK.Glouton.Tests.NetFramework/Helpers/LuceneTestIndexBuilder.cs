using CK.Core;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.IO;

namespace CK.Glouton.Tests
{
    public class LuceneTestIndexBuilder
    {
        public static void ConstructIndex()
        {
            if (_initialized) return;

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
            _initialized = true;
        }

        static bool _initialized;

        private const int LuceneMaxSearch = 10;
        private static readonly string LucenePath = Path.Combine(TestHelper.GetTestLogDirectory(), "Lucene");

        private static readonly LuceneGloutonHandlerConfiguration LuceneGloutonHandlerConfiguration = new LuceneGloutonHandlerConfiguration
        {
            MaxSearch = LuceneMaxSearch,
            Path = LucenePath,
            OpenMode = OpenMode.CREATE,
            Directory = ""
        };

        public static AggregateException ThrowAggregateException(int numberOfException)
        {
            List<Exception> exceptions = new List<Exception>();
            for (int i = 0; i < numberOfException; i++)
            {
                try
                { throw new Exception(); }
                catch (Exception ex) { exceptions.Add(ex); }
            }

            return new AggregateException("Aggregate exceptions list", exceptions);
        }
    }
}
