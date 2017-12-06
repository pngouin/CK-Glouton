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

        /// <summary>
        /// Basic Searcher in a Lucene index for CK.Monitoring log.
        /// </summary>
        /// <param name="luceneConfiguration"></param>
        /// <param name="fields"></param>
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

        /// <summary>
        /// Get all Monitor Id.
        /// </summary>
        public ISet<string> MonitorIdList { get; private set; }

        /// <summary>
        /// Get all App Name.
        /// </summary>
        public ISet<string> AppNameList { get; private set; }

        internal MultiFieldQueryParser QueryParser { get; }

        /// <summary>
        /// Create a query to search into the logs.
        /// </summary>
        /// <param name="monitorId"></param>
        /// <param name="AppName"></param>
        /// <param name="fields"></param>
        /// <param name="logLevel"></param>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Search into Lucene index.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return all log in the Lucene index.
        /// </summary>
        /// <param name="numberDocsToReturn"></param>
        /// <returns></returns>
        public List<ILogViewModel> GetAllLog( int numberDocsToReturn )
        {
            return CreateLogsResult(_indexSearcher?.Search( new WildcardQuery( new Term( "LogLevel", "*" ) ), numberDocsToReturn ) );
        }

        /// <summary>
        /// Get all log exception in the Lucene Index.
        /// </summary>
        /// <param name="numberDocsToReturn"></param>
        /// <returns></returns>
        public List<ILogViewModel> GetAllExceptions( int numberDocsToReturn )
        {
            return CreateLogsResult( _indexSearcher?.Search( _exceptionParser.Parse( "Outer" ), numberDocsToReturn ) );
        }

        /// <summary>
        /// Get the <see cref="TopDocs"/> of a search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
                        throw new ArgumentException(nameof(document));
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
