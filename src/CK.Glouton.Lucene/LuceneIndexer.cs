using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using Lucene.Net.Analysis.Standard;
using CK.Monitoring;
using CK.Core;
using Directory = Lucene.Net.Store.Directory;

namespace CK.Glouton.Lucene
{
    public class LuceneIndexer : IDisposable, IIndexer
    {
        private IndexWriter _writer;
        private DateTimeStamp _lastDateTimeStamp;
        private DateTime _lastCommit;
        private int _numberOfFileToCommit;
        private int _exceptionDepth;
        private LuceneSearcher _searcher;
        private ISet<string> _monitorIdList;
        private ISet<string> _appNameList;

        /// <summary>
        /// Creation of an indexer, it need to be disposed at the end 
        /// to avoid the .lock file to remain in the targeted index
        /// </summary>
        /// <param name="indexDirectoryName">The name of the directory where the indexer will store his indexed file</param>
        /// <param name="indexDirectoryName">The path where will be put the directory, the files will go in the <param name="indexDirectoryName"> </param>
        public LuceneIndexer(string indexDirectoryName, string directory = null)
        {
            string path = indexDirectoryName + "\\" + indexDirectoryName;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            Directory indexDirectory = FSDirectory.Open(new DirectoryInfo(path));

            _writer = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48,
                new StandardAnalyzer(LuceneVersion.LUCENE_48)));
            _lastDateTimeStamp = new DateTimeStamp(DateTime.UtcNow, 0);
            _numberOfFileToCommit = 0;
            _exceptionDepth = 0;
            InitializeIdList();
        }

        public LuceneIndexer()
        {
            Directory indexDirectory = FSDirectory.Open(new DirectoryInfo(LuceneConstant.GetPath()));

            _writer = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48,
                new StandardAnalyzer(LuceneVersion.LUCENE_48)));
            _lastDateTimeStamp = new DateTimeStamp(DateTime.UtcNow, 0);
            _numberOfFileToCommit = 0;
            _exceptionDepth = 0;
            InitializeIdList();
        }

        public LuceneIndexer(string indexDirectoryName)
        {
            Directory indexDirectory = FSDirectory.Open(new DirectoryInfo(LuceneConstant.GetPath(indexDirectoryName)));

            _writer = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48,
                new StandardAnalyzer(LuceneVersion.LUCENE_48)));
            _lastDateTimeStamp = new DateTimeStamp(DateTime.UtcNow, 0);
            _numberOfFileToCommit = 0;
            _exceptionDepth = 0;
            InitializeIdList();
        }

        /// <summary>
        /// Try to initialize a searcher to get the list of monitor and app IDs
        /// It can fail if the index is empty, in this case the searcher is useless
        /// </summary>
        private void InitializeSearcher()
        {
            try
            {
                _searcher = new LuceneSearcher(new string[] { "MonitorIdList", "AppNameList" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Create a Lucene document based on the log given
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName">The app Name given by the Open Block</param>
        /// <returns></returns>
        private Document GetLogDocument(IMulticastLogEntry log, string appName)
        {
            Document document = new Document();
            
            Field monitorId = new StringField("MonitorId", log.MonitorId.ToString(), Field.Store.YES);
            Field groupDepth = new StringField("GroupDepth", log.GroupDepth.ToString(), Field.Store.YES);
            Field previousEntryType = new StringField("PreviousEntryType", log.PreviousEntryType.ToString(), Field.Store.YES);
            Field previousLogTime = new StringField("PreviousLogTime", log.PreviousLogTime.ToString(), Field.Store.YES);
            
            document.Add(monitorId);
            document.Add(groupDepth);
            document.Add(previousEntryType);
            document.Add(previousLogTime);

            if (log.LogType == LogEntryType.Line || log.LogType == LogEntryType.OpenGroup)
            {
                Field logLevel = new TextField("LogLevel", log.LogLevel.ToString(), Field.Store.YES);
                Field text = new TextField("Text", log.Text, Field.Store.YES);
                Field tags = new StringField("Tags", log.Tags.ToString(), Field.Store.YES);
                Field logTime = new StringField("LogTime", DateTools.DateToString(log.LogTime.TimeUtc, DateTools.Resolution.MILLISECOND), Field.Store.YES);
                Field fileName = new TextField("FileName", log.FileName, Field.Store.YES);
                Field lineNumber = new TextField("LineNumber", log.LineNumber.ToString(), Field.Store.YES);
                if (log.Exception != null)
                {
                    Document exDoc = GetExceptionDocuments(log.Exception);
                    Field exception = new TextField("Exception", exDoc.Get("IndexTS").ToString(), Field.Store.YES);
                    document.Add(exception);
                }
                document.Add(logLevel);
                document.Add(text);
                document.Add(tags);
                document.Add(logTime);
                document.Add(fileName);
                document.Add(lineNumber);
            }

            else if (log.LogType == LogEntryType.CloseGroup)
            {
                StringBuilder builder = new StringBuilder();
                foreach(ActivityLogGroupConclusion conclusion in log.Conclusions)
                {
                    builder.Append(conclusion.Text + "\n");
                }
                Field logLevel = new TextField("LogLevel", log.LogLevel.ToString(), Field.Store.YES);
                Field conclusions = new TextField("Conclusions", builder.ToString(), Field.Store.YES);
                Field logTime = new TextField("LogTime", DateTools.DateToString(log.LogTime.TimeUtc, DateTools.Resolution.MILLISECOND), Field.Store.YES);

                document.Add(logLevel);
                document.Add(logTime);
                document.Add(conclusions);
            }

            Field logType = new TextField("LogType", log.LogType.ToString(), Field.Store.YES);
            Field indexTS = new StringField("IndexTS", CreateIndexTS().ToString(), Field.Store.YES);
            Field AppName = new StringField("AppName", appName, Field.Store.YES);

            document.Add(logType);
            document.Add(indexTS);
            document.Add(AppName);

            return document;
        }

        /// <summary>
        /// Create and index a Lucene document based on the exception collected
        /// </summary>
        /// <param name="exception">The exception collected</param>
        /// <returns></returns>
        private Document GetExceptionDocuments(CKExceptionData exception)
        {
            Document document = new Document();

            Field message = new TextField("Message", exception.Message, Field.Store.YES);
            if(exception.StackTrace != null)
            {
                Field stack = new TextField("Stack", exception.StackTrace, Field.Store.YES);
                document.Add(stack);
            }
            Field indexTS = new StringField("IndexTS", CreateIndexTS().ToString(), Field.Store.YES);

            if (exception.AggregatedExceptions != null)
            {
                Field exceptionDepth = new Int32Field("ExceptionDepth", _exceptionDepth, Field.Store.YES);
                document.Add(exceptionDepth);
                if (_exceptionDepth == 0)
                {
                    StringBuilder exList = new StringBuilder();
                    foreach (CKExceptionData ex in exception.AggregatedExceptions)
                    {
                        exList.Append(GetExceptionDocuments(ex).Get("IndexTS"));
                        exList.AppendLine();
                        _exceptionDepth++;
                    }
                    Field aggregatedException = new TextField("AggregatedException", exList.ToString(), Field.Store.YES);
                    document.Add(aggregatedException);
                    _exceptionDepth = 0;
                }
            }

            if (exception.InnerException != null && exception.AggregatedExceptions == null)
            {
                Document exDoc = GetExceptionDocuments(exception.InnerException);
                Field innerException = new StringField("InnerException", exDoc.Get("IndexTS").ToString(), Field.Store.YES);
                document.Add(innerException);
            }

            if (exception.DetailedInfo != null)
            {
                Field details = new TextField("Details", exception.DetailedInfo, Field.Store.YES);
                document.Add(details);
            }
            if (exception.FileName != null)
            {
                Field filename = new StringField("Filename", exception.FileName, Field.Store.YES);
                document.Add(filename);
            }

            document.Add(message);
            document.Add(indexTS);

            _numberOfFileToCommit++;
            _writer.AddDocument(document);
            CommitIfNeeded();

            return document;
        }

        /// <summary>
        /// Create a unique DateTimeStamp to identify each log
        /// </summary>
        /// <returns></returns>
        private DateTimeStamp CreateIndexTS()
        {
            DateTimeStamp IndexTS = new DateTimeStamp(_lastDateTimeStamp, DateTime.UtcNow);
            _lastDateTimeStamp = IndexTS;
            return IndexTS;
        }

        /// <summary>
        /// Check if the Monitor ID and the App ID of a log is already known
        /// if not, it add them to the known list
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName">The app ID given by the Open Block</param>
        private void CheckIds(IMulticastLogEntry log, string appName)
        {
            if (!_monitorIdList.Contains(log.MonitorId.ToString())) _monitorIdList.Add(log.MonitorId.ToString());
            if (!_appNameList.Contains(appName)) _appNameList.Add(appName);
        }

        /// <summary>
        /// Initialize the Monitor ID list and the App ID list
        /// If the Lucene document doesn't exist, it create it
        /// </summary>
        private void InitializeIdList()
        {
            _monitorIdList = new HashSet<string>();
            _appNameList = new HashSet<string>();
            InitializeSearcher();
            if (_searcher == null) return;
            TopDocs hits = _searcher.Search(new WildcardQuery(new Term("MonitorIdList", "*")));
            foreach (ScoreDoc doc in hits.ScoreDocs)
            {
                Document document = _searcher.GetDocument(doc);
                string[] monitorIds = document.Get("MonitorIdList").Split(' ');
                foreach (string id in monitorIds)
                {
                    if (!_monitorIdList.Contains(id) && id != "" && id !=" ") _monitorIdList.Add(id);
                }
                string[] appNames = document.Get("AppNameList").Split(' ');
                foreach (string id in appNames)
                {
                    if (!_appNameList.Contains(id) && id != "" && id != " ") _appNameList.Add(id);
                }
            }
            if (hits.TotalHits == 0) CreateIdListDoc();
        }

        /// <summary>
        /// Index the log document after creating it
        /// </summary>
        /// <param name="log">The log to index</param>
        /// <param name="appName"></param>
        public void IndexLog(IMulticastLogEntry log, string appName)
        {
            CheckIds(log, appName);
            Document document = GetLogDocument(log, appName);
            _writer.AddDocument(document);
            _numberOfFileToCommit++;
            CommitIfNeeded();
        }

        public void IndexLog(ILogEntry log, string appName)
        {
            CheckIds((IMulticastLogEntry)log, appName);
            Document document = GetLogDocument((IMulticastLogEntry)log, appName);
            _writer.AddDocument(document);
            _numberOfFileToCommit++;
            CommitIfNeeded();
        }

        public void IndexLog(ILogEntry entry, IReadOnlyDictionary<string, string> clientData)
        {
            string appName;
            clientData.TryGetValue("AppName", out appName);
            CheckIds((IMulticastLogEntry)entry, appName);
            Document document = GetLogDocument((IMulticastLogEntry)entry, appName);
            _writer.AddDocument(document);
            _numberOfFileToCommit++;
            CommitIfNeeded();
        }

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

        /// <summary>
        /// Get the string containing the monitor ID list, might be big
        /// </summary>
        /// <returns>The string containing the monitor ID list</returns>
        private string GetMonitorIdList ()
        {
            StringBuilder builder = new StringBuilder();
            foreach(string id in _monitorIdList)
            {
                builder.Append(id);
                builder.Append(" ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get the string containing the app ID list
        /// </summary>
        /// <returns>the string containing the app ID list</returns>
        private string GetAppNameList()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string id in _appNameList)
            {
                builder.Append(id);
                builder.Append(" ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Create the Lucene document that contain all Monitor and App ID list
        /// </summary>
        private void CreateIdListDoc()
        {
            Document doc = new Document();

            Field monitorIdList = new TextField("MonitorIdList", GetMonitorIdList(), Field.Store.YES);
            Field appNameList = new TextField("AppNameList", GetAppNameList(), Field.Store.YES);

            doc.Add(monitorIdList);
            doc.Add(appNameList);

            _writer.AddDocument(doc);
        }

        /// <summary>
        /// Update the Lucene document that contain all Monitor and App ID list
        /// </summary>
        private void UpdateIdListDoc()
        {
            Document doc = new Document();

            Field monitorIdList = new TextField("MonitorIdList", GetMonitorIdList(), Field.Store.YES);
            Field appNameList = new TextField("AppNameList", GetAppNameList(), Field.Store.YES);

            doc.Add(monitorIdList);
            doc.Add(appNameList);

            Term term = new Term("MonitorIdList", "*");
            WildcardQuery query = new WildcardQuery(term);
            _writer.DeleteDocuments(query);
            _writer.AddDocument(doc);
        }

        /// <summary>
        /// Commit the change in the index if it's needed
        /// </summary>
        private void CommitIfNeeded()
        {
            if (_numberOfFileToCommit <= 0) return;
            if ((DateTime.UtcNow - _lastCommit).TotalSeconds >= 1 || _lastCommit == null || _numberOfFileToCommit >= 100)
            {
                _writer.Commit();
                _lastCommit = DateTime.UtcNow;
                _numberOfFileToCommit = 0;
            }
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
