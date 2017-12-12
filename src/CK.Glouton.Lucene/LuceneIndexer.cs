using CK.Core;
using CK.Glouton.Model.Logs;
using CK.Monitoring;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
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

        public LuceneIndexer( LuceneConfiguration luceneConfiguration )
        {
            _luceneConfiguration = luceneConfiguration;

            if( !System.IO.Directory.Exists( _luceneConfiguration.ActualPath ) )
                System.IO.Directory.CreateDirectory( _luceneConfiguration.ActualPath );

            Directory indexDirectory = FSDirectory.Open( new DirectoryInfo( _luceneConfiguration.ActualPath ) );
            var config = new IndexWriterConfig( LuceneVersion.LUCENE_48, new StandardAnalyzer( LuceneVersion.LUCENE_48 ) );

            if( luceneConfiguration.OpenMode != null )
                config.OpenMode = (OpenMode)_luceneConfiguration.OpenMode;

            _writer = new IndexWriter( indexDirectory, config );
            _lastDateTimeStamp = new DateTimeStamp( DateTime.UtcNow );
            _numberOfFileToCommit = 0;
            _exceptionDepth = 0;
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
                                exList.Append( ";" );
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

            WriteDocument( document );

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
        /// Index the log document after creating it
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName"></param>
        public void IndexLog( IMulticastLogEntry log, string appName )
        {
            WriteDocument( GetDocument( log, appName ) );
        }

        public void IndexLog( ILogEntry log, IReadOnlyDictionary<string, string> clientData )
        {
            clientData.TryGetValue( LogField.APP_NAME, out var appName );
            IndexLog( (IMulticastLogEntry)log, appName );
        }

        public void WriteDocument( Document document )
        {
            _writer.AddDocument( document );
            _numberOfFileToCommit++;
            CommitIfNeeded();
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
            _writer.Dispose();
        }
    }
}
