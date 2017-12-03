using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CK.Core;
using CK.Monitoring;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
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

            _writer = new IndexWriter( indexDirectory, new IndexWriterConfig( LuceneVersion.LUCENE_48, new StandardAnalyzer( LuceneVersion.LUCENE_48 ) ) );
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
                _searcher = new LuceneSearcher( _luceneConfiguration, new[] { "MonitorIdList", "AppNameList" } );
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        /// <summary>
        /// Create a Lucene document based on the log given
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName">The app Name given by the Open Block</param>
        /// <returns></returns>
        private Document GetLogDocument( IMulticastLogEntry log, string appName )
        {
            var document = new Document();

            Field monitorId = new StringField( "MonitorId", log.MonitorId.ToString(), Field.Store.YES );
            Field groupDepth = new StringField( "GroupDepth", log.GroupDepth.ToString(), Field.Store.YES );
            Field previousEntryType = new StringField( "PreviousEntryType", log.PreviousEntryType.ToString(), Field.Store.YES );
            Field previousLogTime = new StringField( "PreviousLogTime", log.PreviousLogTime.ToString(), Field.Store.YES );

            document.Add( monitorId );
            document.Add( groupDepth );
            document.Add( previousEntryType );
            document.Add( previousLogTime );

            switch( log.LogType )
            {
                case LogEntryType.Line:
                case LogEntryType.OpenGroup:
                    {
                        Field logLevel = new TextField( "LogLevel", log.LogLevel.ToString(), Field.Store.YES );
                        Field text = new TextField( "Text", log.Text, Field.Store.YES );
                        Field tags = new StringField( "Tags", log.Tags.ToString(), Field.Store.YES );
                        Field logTime = new StringField( "LogTime", DateTools.DateToString( log.LogTime.TimeUtc, DateTools.Resolution.MILLISECOND ), Field.Store.YES );
                        Field fileName = new TextField( "FileName", log.FileName, Field.Store.YES );
                        Field lineNumber = new TextField( "LineNumber", log.LineNumber.ToString(), Field.Store.YES );

                        if( log.Exception != null )
                        {
                            var exDoc = GetExceptionDocuments( log.Exception );
                            Field exception = new TextField( "Exception", exDoc.Get( "IndexTS" ), Field.Store.YES );
                            document.Add( exception );
                        }

                        document.Add( logLevel );
                        document.Add( text );
                        document.Add( tags );
                        document.Add( logTime );
                        document.Add( fileName );
                        document.Add( lineNumber );
                        break;
                    }
                case LogEntryType.CloseGroup:
                    {
                        var builder = new StringBuilder();
                        foreach( var conclusion in log.Conclusions )
                            builder.Append( conclusion.Text + "\n" );

                        Field logLevel = new TextField( "LogLevel", log.LogLevel.ToString(), Field.Store.YES );
                        Field conclusions = new TextField( "Conclusions", builder.ToString(), Field.Store.YES );
                        Field logTime = new TextField( "LogTime", DateTools.DateToString( log.LogTime.TimeUtc, DateTools.Resolution.MILLISECOND ), Field.Store.YES );

                        document.Add( logLevel );
                        document.Add( logTime );
                        document.Add( conclusions );
                        break;
                    }
            }

            Field logType = new TextField( "LogType", log.LogType.ToString(), Field.Store.YES );
            Field indexTs = new StringField( "IndexTS", CreateIndexTs().ToString(), Field.Store.YES );
            Field AppName = new StringField( "AppName", appName, Field.Store.YES );

            document.Add( logType );
            document.Add( indexTs );
            document.Add( AppName );

            return document;
        }

        /// <summary>
        /// Create and index a Lucene document based on the exception collected
        /// </summary>
        /// <param name="exception">The exception collected</param>
        /// <returns></returns>
        private Document GetExceptionDocuments( CKExceptionData exception )
        {
            var document = new Document();

            Field message = new TextField( "Message", exception.Message, Field.Store.YES );
            if( exception.StackTrace != null )
            {
                Field stack = new TextField( "Stack", exception.StackTrace, Field.Store.YES );
                document.Add( stack );
            }
            Field indexTs = new StringField( "IndexTS", CreateIndexTs().ToString(), Field.Store.YES );

            if( exception.AggregatedExceptions != null )
            {
                Field exceptionDepth = new Int32Field( "ExceptionDepth", _exceptionDepth, Field.Store.YES );
                document.Add( exceptionDepth );
                if( _exceptionDepth == 0 )
                {
                    var exList = new StringBuilder();
                    foreach( var ex in exception.AggregatedExceptions )
                    {
                        exList.Append( GetExceptionDocuments( ex ).Get( "IndexTS" ) );
                        exList.AppendLine();
                        _exceptionDepth++;
                    }
                    Field aggregatedException = new TextField( "AggregatedException", exList.ToString(), Field.Store.YES );
                    document.Add( aggregatedException );
                    _exceptionDepth = 0;
                }
            }

            if( exception.InnerException != null && exception.AggregatedExceptions == null )
            {
                var exDoc = GetExceptionDocuments( exception.InnerException );
                Field innerException = new StringField( "InnerException", exDoc.Get( "IndexTS" ), Field.Store.YES );
                document.Add( innerException );
            }

            if( exception.DetailedInfo != null )
            {
                Field details = new TextField( "Details", exception.DetailedInfo, Field.Store.YES );
                document.Add( details );
            }

            if( exception.FileName != null )
            {
                Field filename = new StringField( "Filename", exception.FileName, Field.Store.YES );
                document.Add( filename );
            }

            document.Add( message );
            document.Add( indexTs );

            _numberOfFileToCommit++;
            _writer.AddDocument( document );
            CommitIfNeeded();

            return document;
        }

        /// <summary>
        /// Create a unique DateTimeStamp to identify each log
        /// </summary>
        /// <returns></returns>
        private DateTimeStamp CreateIndexTs()
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
            var hits = _searcher.Search( new WildcardQuery( new Term( "MonitorIdList", "*" ) ) );
            foreach( var doc in hits.ScoreDocs )
            {
                var document = _searcher.GetDocument( doc );
                var monitorIds = document.Get( "MonitorIdList" ).Split( ' ' );
                foreach( var id in monitorIds )
                {
                    if( !MonitorIdList.Contains( id ) && id != "" && id != " " )
                        MonitorIdList.Add( id );
                }
                var appNames = document.Get( "AppNameList" ).Split( ' ' );
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
            var document = GetLogDocument( log, appName );
            _writer.AddDocument( document );
            _numberOfFileToCommit++;
            CommitIfNeeded();
        }

        public void IndexLog( ILogEntry log, string appName )
        {
            IndexLog((IMulticastLogEntry)log, appName);
        }

        public void IndexLog( ILogEntry log, IReadOnlyDictionary<string, string> clientData )
        {
            clientData.TryGetValue("AppName", out var appName);
            IndexLog((IMulticastLogEntry)log, appName);
        }

        /*
        /// <summary>
        /// Index the open block of this indexer
        /// </summary>
        /// <param name="openBlock">The open block of this indexer</param>
        /// 
        //public void IndexOpenBlock(IOpen openBlock)
        //{
        //    Document document = GetOpenBlockDocument(openBlock);
        //    _writer.AddDocument(document);
        //    _numberOfFileToCommit++;
        //    CommitIfNeeded();
        //}
        */

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

            Field monitorIdList = new TextField( "MonitorIdList", GetMonitorIdList(), Field.Store.YES );
            Field appNameList = new TextField( "AppNameList", GetAppNameList(), Field.Store.YES );

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

            Field monitorIdList = new TextField( "MonitorIdList", GetMonitorIdList(), Field.Store.YES );
            Field appNameList = new TextField( "AppNameList", GetAppNameList(), Field.Store.YES );

            doc.Add( monitorIdList );
            doc.Add( appNameList );

            var term = new Term( "MonitorIdList", "*" );
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
