using CK.Glouton.Model.Logs;
using CK.Glouton.Model.Lucene;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directory = Lucene.Net.Store.Directory;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcher : ILuceneSearcher
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

        public List<ILogViewModel> Search( string searchQuery )
        {
            return CreateLogsResult(_indexSearcher?.Search( QueryParser.Parse( searchQuery ), _luceneConfiguration.MaxSearch ) );
        }

        public List<ILogViewModel> Search( Query searchQuery )
        {
            return CreateLogsResult(_indexSearcher?.Search( searchQuery, _luceneConfiguration.MaxSearch ) );
        }

        public Document GetDocument( ScoreDoc scoreDoc )
        {
            return _indexSearcher?.Doc( scoreDoc.Doc );
        }

        public Document GetDocument(Query query)
        {
            return GetDocument(_indexSearcher?.Search(query, _luceneConfiguration.MaxSearch).ScoreDocs.First());
        }

        public List<ILogViewModel> GetAllLog( int numberDocsToReturn )
        {
            return CreateLogsResult(_indexSearcher?.Search( new WildcardQuery( new Term( "LogLevel", "*" ) ), numberDocsToReturn ) );
        }

        public List<ILogViewModel> GetAllExceptions( int numberDocsToReturn )
        {
            return CreateLogsResult( _indexSearcher?.Search( _exceptionParser.Parse( "Outer" ), numberDocsToReturn ) );
        }

        public TopDocs QuerySearch ( Query query ) // TODO: Get a good name
        {
            return _indexSearcher?.Search(query, _luceneConfiguration.MaxSearch);
        }

        private List<ILogViewModel> CreateLogsResult ( TopDocs topDocs )
        {
            var result = new List<ILogViewModel>();
            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var document = GetDocument(scoreDoc);
                switch (document.Get("LogType"))
                {
                    case "Line":
                        result.Add( LineViewModel.Get(this, document) );
                        break;
                    case "CloseGroup":
                        result.Add( CloseGroupViewModel.Get(this, document) );
                        break;
                    case "OpenGroup":
                        result.Add( OpenGroupViewModel.Get(this, document) );
                        break;
                    default:
                        throw new InvalidOperationException("LogType not reconize is the document.");
                }
            }

            return result;
        }

        private void InitializeIdList()
        {
            MonitorIdList = new HashSet<string>();
            AppNameList = new HashSet<string>();
            var hits = QuerySearch( new WildcardQuery( new Term( "MonitorIdList", "*" ) ) );

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
    }
}
