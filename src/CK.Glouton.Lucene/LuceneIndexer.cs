using CK.Core;
using CK.Glouton.Model.Logs;
using CK.Monitoring;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Directory = Lucene.Net.Store.Directory;

namespace CK.Glouton.Lucene
{
    public class LuceneIndexer : IDisposable, IIndexer
    {
        private readonly IndexWriter _writer;
        private readonly LuceneConfiguration _luceneConfiguration;

        private DateTimeStamp _lastDateTimeStamp;
        private DateTime _lastCommit;
        private int _numberOfFileToCommit;
        private int _exceptionDepth;
        private LuceneSearcher _searcher;

        public LuceneIndexer( LuceneConfiguration luceneConfiguration )
        {
            _luceneConfiguration = luceneConfiguration;

            if( !System.IO.Directory.Exists( _luceneConfiguration.ActualPath ) )
                System.IO.Directory.CreateDirectory( _luceneConfiguration.ActualPath );

            Directory indexDirectory = FSDirectory.Open( new DirectoryInfo( _luceneConfiguration.ActualPath ) );
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, new StandardAnalyzer(LuceneVersion.LUCENE_48));

            if (luceneConfiguration.OpenMode != null)
                config.OpenMode = (OpenMode)_luceneConfiguration.OpenMode;

            _writer = new IndexWriter( indexDirectory, config);
            _lastDateTimeStamp = new DateTimeStamp( DateTime.UtcNow );
            _numberOfFileToCommit = 0;
            _exceptionDepth = 0;

            InitializeIdList();
        }

        public ISet<string> AppNameList { get; private set; }

        public ISet<string> MonitorIdList { get; private set; }


        /// <summary>
        /// Try to initialize a searcher to get the list of monitor and app IDs
        /// It can fail if the index is empty, in this case the searcher is useless
        /// </summary>
        private void InitializeSearcher()
        {
            var file = new DirectoryInfo( _luceneConfiguration.ActualPath ).EnumerateFiles();
            if( !file.Any() )
                return;

            try
            {
                _searcher = new LuceneSearcher( _luceneConfiguration, new[] { LogField.MONITOR_ID_LIST, LogField.APP_NAME } );
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        private Document GetDocument( IMulticastLogEntry log, string appName )
        {
            var document = new Document();

            foreach( var propertyInfo in log.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
            {
                var logValue = log.GetType().GetProperty( propertyInfo.Name )?.GetValue( log );
                if( logValue == null )
                    continue;

                switch( propertyInfo.PropertyType.Name )
                {
                    case LogField.DATE_TIME_STAMP:
                        document.Add( new TextField
                            (
                                propertyInfo.Name,
                                DateTools.DateToString( ( logValue as DateTimeStamp? ?? new DateTimeStamp( new DateTime( 1, 1, 1 ) ) ).TimeUtc, DateTools.Resolution.MILLISECOND ),
                                Field.Store.YES
                            ) );
                        break;

                    case "List`1": // Matches: IReadOnlyList<ActivityLogGroupConclusion>
                        var stringBuilder = new StringBuilder();
                        foreach( var conclusion in log.Conclusions )
                            stringBuilder.Append( conclusion.Text + "\n" );
                        document.Add( new TextField
                            (
                                propertyInfo.Name,
                                stringBuilder.ToString(),
                                Field.Store.YES
                            ) );
                        break;

                    case LogField.CK_EXCEPTION_DATA:
                        document.Add( new TextField(
                                "Exception",
                                GetDocument( logValue as CKExceptionData ).Get( "IndexDTS" ),
                                Field.Store.YES
                            ) );
                        break;

                    case LogField.CK_TRAIT:
                        document.Add( new StringField(
                            propertyInfo.Name,
                            logValue.ToString(),
                            Field.Store.YES
                            ) );
                        break;

                    default:
                        document.Add( new TextField( propertyInfo.Name, logValue.ToString(), Field.Store.YES ) );
                        break;
                }
            }

            document.Add( new TextField( LogField.INDEX_DTS, CreateIndexDts().ToString(), Field.Store.YES ) );
            document.Add( new TextField( LogField.APP_NAME, appName, Field.Store.YES ) );

            return document;
        }

        private Document GetDocument( CKExceptionData exception )
        {
            var document = new Document();

            foreach( var propertyInfo in exception.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
            {
                var exceptionValue = exception.GetType().GetProperty( propertyInfo.Name )?.GetValue( exception );
                if( exceptionValue == null )
                    continue;

                switch( propertyInfo.Name )
                {
                    case LogField.AGGREGATED_EXCEPTIONS:
                        if( _exceptionDepth == 0 )
                        {
                            var exList = new StringBuilder();
                            foreach( var ex in exception.AggregatedExceptions )
                            {
                                exList.Append( GetDocument( ex ).Get( LogField.INDEX_DTS ) );
                                exList.AppendLine();
                                _exceptionDepth++;
                            }
                            document.Add( new Int32Field( LogField.EXCEPTION_DEPTH, _exceptionDepth, Field.Store.YES ) );
                            document.Add( new TextField( LogField.AGGREGATED_EXCEPTIONS, exList.ToString(), Field.Store.YES ) );
                            _exceptionDepth = 0;
                        }

                        break;
                    case LogField.INNER_EXCEPTION:
                        document.Add( new StringField( LogField.INNER_EXCEPTION, GetDocument( exceptionValue as CKExceptionData ).Get( "IndexDTS" ), Field.Store.YES ) );
                        break;
                    default:
                        document.Add( new TextField( propertyInfo.Name, exceptionValue.ToString(), Field.Store.YES ) );
                        break;
                }
            }

            document.Add( new StringField( LogField.INDEX_DTS, CreateIndexDts().ToString(), Field.Store.YES ) );

            return document;
        }

        /// <summary>
        /// Create a unique DateTimeStamp to identify each log
        /// </summary>
        /// <returns></returns>
        private DateTimeStamp CreateIndexDts()
        {
            var indexTs = new DateTimeStamp( _lastDateTimeStamp, DateTime.UtcNow );
            _lastDateTimeStamp = indexTs;
            return indexTs;
        }

        /// <summary>
        /// Check if the Monitor ID and the App ID of a log is already known
        /// if not, it add them to the known list
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName">The app ID given by the Open Block</param>
        private void CheckIds( IMulticastLogEntry log, string appName )
        {
            if( !MonitorIdList.Contains( log.MonitorId.ToString() ) )
                MonitorIdList.Add( log.MonitorId.ToString() );

            if( !AppNameList.Contains( appName ) )
                AppNameList.Add( appName );
        }

        /// <summary>
        /// Initialize the Monitor ID list and the App ID list
        /// If the Lucene document doesn't exist, it create it
        /// </summary>
        private void InitializeIdList()
        {
            MonitorIdList = new HashSet<string>();
            AppNameList = new HashSet<string>();
            InitializeSearcher();
            if( _searcher == null )
                return;
            var hits = _searcher.QuerySearch( new WildcardQuery( new Term( LogField.MONITOR_ID_LIST, "*" ) ) );
            foreach( var doc in hits.ScoreDocs )
            {
                var document = _searcher.GetDocument( doc );
                var monitorIds = document.Get( LogField.MONITOR_ID_LIST ).Split( ' ' );
                foreach( var id in monitorIds )
                {
                    if( !MonitorIdList.Contains( id ) && id != "" && id != " " )
                        MonitorIdList.Add( id );
                }
                var appNames = document.Get( LogField.APP_NAME_LIST ).Split( ' ' );
                foreach( var id in appNames )
                {
                    if( !AppNameList.Contains( id ) && id != "" && id != " " )
                        AppNameList.Add( id );
                }
            }
            if( hits.TotalHits == 0 )
                CreateIdListDoc();
        }

        /// <summary>
        /// Index the log document after creating it
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName"></param>
        public void IndexLog( IMulticastLogEntry log, string appName )
        {
            CheckIds( log, appName );
            var document = GetDocument( log, appName );
            _writer.AddDocument( document );
            _numberOfFileToCommit++;
            CommitIfNeeded();
        }

        public void IndexLog( ILogEntry log, string appName )
        {
            IndexLog( (IMulticastLogEntry)log, appName );
        }

        public void IndexLog( ILogEntry log, IReadOnlyDictionary<string, string> clientData )
        {
            clientData.TryGetValue( LogField.APP_NAME, out var appName );
            IndexLog( (IMulticastLogEntry)log, appName );
        }

        /// <summary>
        /// Get the string containing the monitor ID list, might be big
        /// </summary>
        /// <returns>The string containing the monitor ID list</returns>
        private string GetMonitorIdList()
        {
            var builder = new StringBuilder();
            foreach( var id in MonitorIdList )
            {
                builder.Append( id );
                builder.Append( " " );
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get the string containing the app ID list
        /// </summary>
        /// <returns>the string containing the app ID list</returns>
        private string GetAppNameList()
        {
            var builder = new StringBuilder();
            foreach( var id in AppNameList )
            {
                builder.Append( id );
                builder.Append( " " );
            }
            return builder.ToString();
        }

        /// <summary>
        /// Create the Lucene document that contain all Monitor and App ID list
        /// </summary>
        private void CreateIdListDoc()
        {
            var doc = new Document();

            Field monitorIdList = new TextField( LogField.MONITOR_ID_LIST, GetMonitorIdList(), Field.Store.YES );
            Field appNameList = new TextField( LogField.APP_NAME_LIST, GetAppNameList(), Field.Store.YES );

            doc.Add( monitorIdList );
            doc.Add( appNameList );

            _writer.AddDocument( doc );
        }

        /// <summary>
        /// Update the Lucene document that contain all Monitor and App ID list
        /// </summary>
        private void UpdateIdListDoc()
        {
            var doc = new Document();

            Field monitorIdList = new TextField( LogField.MONITOR_ID_LIST, GetMonitorIdList(), Field.Store.YES );
            Field appNameList = new TextField( LogField.APP_NAME_LIST, GetAppNameList(), Field.Store.YES );

            doc.Add( monitorIdList );
            doc.Add( appNameList );

            var term = new Term( LogField.MONITOR_ID_LIST, "*" );
            var query = new WildcardQuery( term );
            _writer.DeleteDocuments( query );
            _writer.AddDocument( doc );
        }

        /// <summary>
        /// Commit the change in the index if it's needed
        /// </summary>
        private void CommitIfNeeded()
        {
            if( _numberOfFileToCommit <= 0 )
                return;

            if( !( ( DateTime.UtcNow - _lastCommit ).TotalSeconds >= 1 ) && _lastCommit != null && _numberOfFileToCommit < 100 )
                return;

            _writer.Commit();
            _lastCommit = DateTime.UtcNow;
            _numberOfFileToCommit = 0;
        }

        /// <summary>
        /// Force the commit
        /// </summary>
        public void ForceCommit()
        {
            _writer.Commit();
        }

        /// <summary>
        /// Dispose the indexer
        /// </summary>
        public void Dispose()
        {
            UpdateIdListDoc();
            _writer.Dispose();
        }
    }
}
