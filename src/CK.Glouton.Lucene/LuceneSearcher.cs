using System;
using System.Collections.Generic;
using System.Linq;
using CK.Glouton.Model.Logs;
using CK.Glouton.Model.Lucene;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcher : ILuceneSearcher
    {
        private readonly IndexSearcher _indexSearcher;
        private readonly Sort _sort;

        /// <summary>
        /// Basic Searcher in a Lucene index for CK.Monitoring log.
        /// </summary>
        /// <param name="multiReader"></param>
        public LuceneSearcher( MultiReader multiReader )
        {
            _indexSearcher = new IndexSearcher( multiReader );
            _sort = new Sort(
                SortField.FIELD_SCORE,
                new SortField( LogField.LOG_TIME, SortFieldType.STRING ) );
        }

        /// <summary>
        /// Search into Lucene index.
        /// If the <see cref="LuceneSearcherConfiguration"/> is not correct return null.
        /// </summary>
        /// <param name="searchConfiguration"></param>
        /// <returns></returns>
        public List<ILogViewModel> Search( LuceneSearcherConfiguration searchConfiguration )
        {
            if( !CheckSearchConfiguration( searchConfiguration ) )
                return null;

            if( searchConfiguration.ESearchMethod == ESearchMethod.FullText )
            {
                return Search( searchConfiguration, new MultiFieldQueryParser
                (
                    LuceneVersion.LUCENE_48,
                    searchConfiguration.Fields,
                    new StandardAnalyzer( LuceneVersion.LUCENE_48 )
                ).Parse( searchConfiguration.Query ) );
            }

            return searchConfiguration.WantAll
                ? Search( searchConfiguration, GetAll( searchConfiguration.All ) )
                : CreateLogsResult( _indexSearcher?.Search( CreateQuery( searchConfiguration ), searchConfiguration.MaxResult, _sort ) );
        }

        public int SearchCount( LuceneSearcherConfiguration luceneSearcherConfiguration )
        {
            if( !CheckSearchConfiguration( luceneSearcherConfiguration ) )
                return -1;

            if( luceneSearcherConfiguration.ESearchMethod == ESearchMethod.FullText )
            {
                return _indexSearcher?.Search( new MultiFieldQueryParser
                (
                    LuceneVersion.LUCENE_48,
                    luceneSearcherConfiguration.Fields,
                    new StandardAnalyzer( LuceneVersion.LUCENE_48 )
                ).Parse( luceneSearcherConfiguration.Query ), luceneSearcherConfiguration.MaxResult )
                 .TotalHits ?? -1;
            }

            if( luceneSearcherConfiguration.WantAll )
                return _indexSearcher?.Search( GetAll( luceneSearcherConfiguration.All ), luceneSearcherConfiguration.MaxResult ).TotalHits ?? -1;

            return _indexSearcher?.Search( CreateQuery( luceneSearcherConfiguration ), luceneSearcherConfiguration.MaxResult ).TotalHits ?? -1;
        }

        private static Query GetAll( ELuceneWantAll all )
        {
            var query = new BooleanQuery
            {
                {
                    all == ELuceneWantAll.Exception
                        ? new WildcardQuery( new Term( LogField.EXCEPTION, "*" ) )
                        : new WildcardQuery( new Term( LogField.LOG_LEVEL, "*" ) ),
                    Occur.MUST
                }
            };

            return query;
        }

        /// <summary>
        /// Create a query to search into the logs.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static Query CreateQuery( ILuceneSearcherConfiguration configuration )
        {
            var bQuery = new BooleanQuery();

            if( configuration.MonitorId != null )
                bQuery.Add( CreateMonitorIdQuery( configuration ), Occur.MUST );
            if( configuration.DateEnd.Year != 1 && configuration.DateStart.Year != 1 )
                bQuery.Add( CreateTimeQuery( configuration ), Occur.MUST );
            if( configuration.LogLevel != null )
                bQuery.Add( CreateLogLevelQuery( configuration ), Occur.MUST );
            if( configuration.GroupDepth != null )
                bQuery.Add( CreateGroupDepthQuery( configuration ), Occur.MUST );

            bQuery.Add( CreateFieldQuery( configuration ), Occur.MUST );

            return bQuery;
        }

        private static Query CreateGroupDepthQuery( ILuceneSearcherConfiguration configuration )
        {
            return new WildcardQuery( new Term( LogField.GROUP_DEPTH, configuration.GroupDepth.ToString() ) );
        }

        private static Query CreateTimeQuery( ILuceneSearcherConfiguration configuration )
        {
            return TermRangeQuery.NewStringRange
            (
                LogField.LOG_TIME,
                BuildDate( configuration.DateStart ),
                BuildDate( configuration.DateEnd ),
                true,
                true
            );
        }

        private static string BuildDate( DateTime dateTime ) => DateTools.DateToString( dateTime, DateTools.Resolution.MILLISECOND );

        private static Query CreateLogLevelQuery( ILuceneSearcherConfiguration configuration )
        {
            var levelParser = new QueryParser
            (
                LuceneVersion.LUCENE_48,
                LogField.LOG_LEVEL,
                new StandardAnalyzer( LuceneVersion.LUCENE_48 )
            );

            var bLevelQuery = new BooleanQuery();
            foreach( var level in configuration.LogLevel )
            {
                bLevelQuery.Add( levelParser.Parse( level ), Occur.SHOULD );
            }

            return bLevelQuery;
        }

        private static Query CreateMonitorIdQuery( ILuceneSearcherConfiguration configuration )
        {
            return new TermQuery( new Term( LogField.MONITOR_ID, configuration.MonitorId ) );
        }

        private static Query CreateFieldQuery( ILuceneSearcherConfiguration configuration )
        {
            var bFieldQuery = new BooleanQuery();

            if( configuration.Fields == null || configuration.Fields.Length == 0 )
            {
                bFieldQuery.Add( new WildcardQuery( new Term( LogField.LOG_LEVEL, "*" ) ), Occur.SHOULD );
                return bFieldQuery;
            }

            foreach( var field in configuration.Fields )
            {
                if( field == LogField.TEXT && configuration.Query != null )
                    bFieldQuery.Add( new QueryParser( LuceneVersion.LUCENE_48, field, new StandardAnalyzer( LuceneVersion.LUCENE_48 ) ).Parse( configuration.Query ), Occur.SHOULD );
                else
                {
                    bFieldQuery.Add( new WildcardQuery( new Term( field, configuration.Query ?? "*" ) ), Occur.SHOULD );
                }
            }

            return bFieldQuery;
        }

        private List<ILogViewModel> Search( ILuceneSearcherConfiguration configuration, Query searchQuery )
        {
            return CreateLogsResult( _indexSearcher?.Search( searchQuery, configuration.MaxResult, _sort ) );
        }

        public Document GetDocument( ScoreDoc scoreDoc )
        {
            return _indexSearcher?.Doc( scoreDoc.Doc );
        }

        public Document GetDocument( Query query, int maxResult )
        {
            return GetDocument( _indexSearcher?.Search( query, maxResult ).ScoreDocs.First() );
        }

        public Document GetDocument( string key, string value, int maxResult )
        {
            return GetDocument( _indexSearcher?.Search( new TermQuery( new Term( key, value ) ), maxResult, _sort ).ScoreDocs.First() );
        }

        private bool CheckSearchConfiguration( LuceneSearcherConfiguration configuration )
        {
            if( configuration == null )
                throw new ArgumentNullException( nameof( configuration ) );

            if( configuration.MaxResult == 0 )
                throw new ArgumentException( nameof( configuration ) );
            return true;
        }

        /// <summary>
        /// Get all monitor id in all AppName.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetAllMonitorId()
        {
            var hits = _indexSearcher.Search( new WildcardQuery( new Term( LogField.MONITOR_ID, "*" ) ), int.MaxValue );
            var monitorIds = new HashSet<string>();
            foreach( var doc in hits.ScoreDocs )
            {
                var monitorId = GetDocument( doc ).Get( LogField.MONITOR_ID );
                monitorIds.Add( monitorId );
            }

            return monitorIds;
        }

        private List<ILogViewModel> CreateLogsResult( TopDocs topDocs )
        {
            var result = new List<ILogViewModel>();
            foreach( var scoreDoc in topDocs.ScoreDocs )
            {
                var document = GetDocument( scoreDoc );
                switch( document.Get( LogField.LOG_TYPE ) )
                {
                    case "Line":
                        result.Add( LineViewModel.Get( this, document ) );
                        break;
                    case "CloseGroup":
                        result.Add( CloseGroupViewModel.Get( this, document ) );
                        break;
                    case "OpenGroup":
                        result.Add( OpenGroupViewModel.Get( this, document ) );
                        break;
                    default:
                        throw new ArgumentException( nameof( document ) );
                }
            }

            return result;
        }
    }
}