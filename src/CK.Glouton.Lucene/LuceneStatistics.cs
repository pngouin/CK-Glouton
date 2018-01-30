using CK.Glouton.Model.Lucene.Searcher;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Lucene
{
    public class LuceneStatistics
    {
        private readonly LuceneSearcherManager _luceneSearcher;
        private readonly LuceneSearcherConfiguration _allSearchLog;
        private readonly LuceneSearcherConfiguration _allSearchException;

        public LuceneStatistics( LuceneConfiguration configuration )
        {
            _luceneSearcher = new LuceneSearcherManager( configuration );
            _allSearchLog = new LuceneSearcherConfiguration
            {
                MaxResult = int.MaxValue
            };
            _allSearchLog.SearchAll( ELuceneWantAll.Log );

            _allSearchException = new LuceneSearcherConfiguration
            {
                MaxResult = int.MaxValue
            };
            _allSearchException.SearchAll( ELuceneWantAll.Exception );
        }


        public int LogInAppNameCount( string appName )
        {
            return _luceneSearcher.GetSearcher( appName )?.SearchCount( _allSearchLog ) ?? -1;
        }

        public int ExceptionInAppNameCount( string appName )
        {
            return _luceneSearcher.GetSearcher( appName )?.SearchCount( _allSearchException ) ?? -1;
        }

        public int AllLogCount => _luceneSearcher.GetSearcher( _luceneSearcher.AppName.ToArray() ).SearchCount( _allSearchLog );

        public int AllExceptionCount => _luceneSearcher.GetSearcher( _luceneSearcher.AppName.ToArray() ).SearchCount( _allSearchException );

        public IEnumerable<string> GetAppNames => _luceneSearcher.AppName;
        public int AppNameCount => _luceneSearcher.AppName.Count;

    }
}
