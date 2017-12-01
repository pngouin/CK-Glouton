using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcher
    {
        private readonly LuceneConfiguration _luceneConfiguration;

        private readonly IndexSearcher _indexSearcher;
        private readonly QueryParser _exceptionParser;
        private readonly QueryParser _levelParser;

        private Query _query;

        public LuceneSearcher( LuceneConfiguration luceneConfiguration, string[] fields )
        {
            _luceneConfiguration = luceneConfiguration;

            var file = new DirectoryInfo( luceneConfiguration.ActualPath ).EnumerateFiles();
            if( !file.Any() )
                return;

            Directory indexDirectory = FSDirectory.Open( new DirectoryInfo( luceneConfiguration.ActualPath ) );
            _indexSearcher = new IndexSearcher( DirectoryReader.Open( indexDirectory ) );
            QueryParser = new MultiFieldQueryParser( LuceneVersion.LUCENE_48,
                fields,
                new StandardAnalyzer( LuceneVersion.LUCENE_48 ) );
            _exceptionParser = new QueryParser( LuceneVersion.LUCENE_48,
                "Message",
                new StandardAnalyzer( LuceneVersion.LUCENE_48 ) );
            _levelParser = new QueryParser( LuceneVersion.LUCENE_48,
                "LogLevel",
                new StandardAnalyzer( LuceneVersion.LUCENE_48 ) );
            InitializeIdList();
        }


        public ISet<string> MonitorIdList { get; private set; }

        public ISet<string> AppNameList { get; private set; }

        internal MultiFieldQueryParser QueryParser { get; }

        public Query CreateQuery( string monitorId, string AppName, string[] fields, string[] logLevel, DateTime startingDate, DateTime endingDate, string searchQuery )
        {
            var bQuery = new BooleanQuery();
            if( monitorId != "All" )
                bQuery.Add( new TermQuery( new Term( "MonitorId", monitorId ) ), Occur.MUST );
            if( AppName != "All" )
                bQuery.Add( new TermQuery( new Term( "AppName", AppName ) ), Occur.MUST );
            var bFieldQuery = new BooleanQuery();
            foreach( var field in fields )
            {
                if( field == "Text" && searchQuery != "*" )
                    bFieldQuery.Add( new QueryParser( LuceneVersion.LUCENE_48, field, new StandardAnalyzer( LuceneVersion.LUCENE_48 ) ).Parse( searchQuery ), Occur.SHOULD );
                else
                    bFieldQuery.Add( new WildcardQuery( new Term( field, searchQuery ) ), Occur.SHOULD );
            }
            bQuery.Add( bFieldQuery, Occur.MUST );
            var bLevelQuery = new BooleanQuery();
            foreach( var level in logLevel )
            {
                bLevelQuery.Add( _levelParser.Parse( level ), Occur.SHOULD );
            }
            bQuery.Add( bLevelQuery, Occur.MUST );
            bQuery.Add( new TermRangeQuery( "LogTime",
                new BytesRef( DateTools.DateToString( startingDate, DateTools.Resolution.MILLISECOND ) ),
                new BytesRef( DateTools.DateToString( endingDate, DateTools.Resolution.MILLISECOND ) ),
                includeLower: true,
                includeUpper: true ), Occur.MUST );
            return bQuery;
        }

        public TopDocs Search( string searchQuery )
        {
            if( !CheckSearcher( _indexSearcher ) )
                return null;
            _query = QueryParser.Parse( searchQuery );
            return _indexSearcher.Search( _query, _luceneConfiguration.MaxSearch );
        }

        public TopDocs Search( Query searchQuery )
        {
            return !CheckSearcher( _indexSearcher ) ? null : _indexSearcher.Search( searchQuery, _luceneConfiguration.MaxSearch );
        }

        public Document GetDocument( ScoreDoc scoreDoc )
        {
            return !CheckSearcher( _indexSearcher ) ? null : _indexSearcher.Doc( scoreDoc.Doc );
        }

        public TopDocs GetAllLog( int numberDocsToReturn )
        {
            return !CheckSearcher( _indexSearcher ) ? null : _indexSearcher.Search( new WildcardQuery( new Term( "LogLevel", "*" ) ), numberDocsToReturn );
        }

        public TopDocs GetAllExceptions( int numberDocsToReturn )
        {
            return !CheckSearcher( _indexSearcher ) ? null : _indexSearcher.Search( _exceptionParser.Parse( "Outer" ), numberDocsToReturn );
        }

        private void InitializeIdList()
        {
            MonitorIdList = new HashSet<string>();
            AppNameList = new HashSet<string>();
            var hits = Search( new WildcardQuery( new Term( "MonitorIdList", "*" ) ) );
            foreach( var doc in hits.ScoreDocs )
            {
                var document = GetDocument( doc );
                var monitorIds = document.Get( "MonitorIdList" ).Split( ' ' );
                foreach( var id in monitorIds )
                {
                    if( !MonitorIdList.Contains( id ) )
                        MonitorIdList.Add( id );
                }
                var appName = document.Get( "AppNameList" ).Split( ' ' );
                foreach( var id in appName )
                {
                    if( !AppNameList.Contains( id ) )
                        AppNameList.Add( id );
                }
            }
        }

        private static bool CheckSearcher( IndexSearcher searcher )
        {
            return searcher != null;
        }
    }
}
