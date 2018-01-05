﻿using CK.Glouton.Model.Lucene;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Lucene
{
    public class LuceneStatistics
    {
        LuceneSearcherManager _luceneSearcher;
        LuceneSearcherConfiguration _allSearchLog;
        LuceneSearcherConfiguration _allSearchException;

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
            if( !_luceneSearcher.AppName.Contains( appName ) )
                return 0;
            return _luceneSearcher.GetSearcher( appName ).Search( _allSearchLog ).Count;
        }

        public int ExceptionInAppNameCount( string appName )
        {
            if( !_luceneSearcher.AppName.Contains( appName ) )
                return 0;
            return _luceneSearcher.GetSearcher( appName ).Search( _allSearchException ).Count;
        }

        public int AllLogCount => _luceneSearcher.GetSearcher( _luceneSearcher.AppName.ToArray() ).Search( _allSearchLog ).Count;

        public int AllExceptionCount => _luceneSearcher.GetSearcher( _luceneSearcher.AppName.ToArray() ).Search( _allSearchException ).Count;

        public IEnumerable<string> GetAppNames => _luceneSearcher.AppName;
        public int AppNameCount => _luceneSearcher.AppName.Count;

    }
}
