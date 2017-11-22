using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Store;
using System.Linq;
using Directory = Lucene.Net.Store.Directory;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcher
    {
        IndexSearcher _indexSearcher;
        MultiFieldQueryParser _queryParser;
        QueryParser _exceptionParser;
        QueryParser _levelParser;
        Query _query;
        private ISet<string> _monitorIdList;
        private ISet<string> _appNameList;

        public LuceneSearcher(string[] fields)
        {
            var file = new DirectoryInfo(LuceneConstant.GetPath()).EnumerateFiles();
            if (!file.Any()) return;
            Directory indexDirectory = FSDirectory.Open(new DirectoryInfo(LuceneConstant.GetPath()));
            _indexSearcher = new IndexSearcher(DirectoryReader.Open(indexDirectory));
            _queryParser =  new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                fields,
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            _exceptionParser = new QueryParser(LuceneVersion.LUCENE_48,
                "Message",
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            _levelParser = new QueryParser(LuceneVersion.LUCENE_48,
                "LogLevel",
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            InitializeIdList();
        }

        public LuceneSearcher(string dir, string[] fields)
        {
            var file = new DirectoryInfo(LuceneConstant.GetPath(dir)).EnumerateFiles();
            if (!file.Any()) return;
            Directory indexDirectory = FSDirectory.Open(new DirectoryInfo(LuceneConstant.GetPath(dir)));
            _indexSearcher = new IndexSearcher(DirectoryReader.Open(indexDirectory));
            _queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                fields,
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            _exceptionParser = new QueryParser(LuceneVersion.LUCENE_48,
                "Message",
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            _levelParser = new QueryParser(LuceneVersion.LUCENE_48,
                "LogLevel",
                new StandardAnalyzer(LuceneVersion.LUCENE_48));
            InitializeIdList();
        }

        public ISet<string> MonitorIdList => _monitorIdList;

        public ISet<string> AppNameList => _appNameList;

        internal MultiFieldQueryParser QueryParser => _queryParser;

        public Query CreateQuery(string monitorID, string AppName, string[] fields, string[] logLevel, DateTime startingDate, DateTime endingDate, string searchQuery)
        {
            BooleanQuery bQuery = new BooleanQuery();
            if (monitorID != "All") bQuery.Add(new TermQuery(new Term("MonitorId", monitorID)), Occur.MUST);
            if (AppName != "All") bQuery.Add(new TermQuery(new Term("AppName", AppName)), Occur.MUST);
            BooleanQuery bFieldQuery = new BooleanQuery();
            foreach (string field in fields)
            {
                if(field == "Text" && searchQuery != "*")
                    bFieldQuery.Add(new QueryParser(LuceneVersion.LUCENE_48, field, new StandardAnalyzer(LuceneVersion.LUCENE_48)).Parse(searchQuery), Occur.SHOULD);
                else
                    bFieldQuery.Add(new WildcardQuery(new Term(field, searchQuery)), Occur.SHOULD);
            }
            bQuery.Add(bFieldQuery, Occur.MUST);
            BooleanQuery bLevelQuery = new BooleanQuery();
            foreach (string level in logLevel)
            {
                bLevelQuery.Add(_levelParser.Parse(level), Occur.SHOULD);
            }
            bQuery.Add(bLevelQuery, Occur.MUST);
            bQuery.Add(new TermRangeQuery("LogTime",
                new BytesRef(DateTools.DateToString(startingDate, DateTools.Resolution.MILLISECOND)),
                new BytesRef(DateTools.DateToString(endingDate, DateTools.Resolution.MILLISECOND)),
                includeLower: true,
                includeUpper: true), Occur.MUST);
            return bQuery;
        }

        public TopDocs Search (string searchQuery)
        {
            if (!CheckSearcher(_indexSearcher)) return null;
            _query = _queryParser.Parse(searchQuery);
            return _indexSearcher.Search(_query, LuceneConstant.MaxSearch);
        }

        public TopDocs Search(Query searchQuery)
        {
            if (!CheckSearcher(_indexSearcher)) return null;
            return _indexSearcher.Search(searchQuery, LuceneConstant.MaxSearch);
        }

        public Document GetDocument(ScoreDoc scoreDoc)
        {
            if (!CheckSearcher(_indexSearcher)) return null;
            return _indexSearcher.Doc(scoreDoc.Doc);
        }

        public TopDocs GetAllLog(int numberDocsToReturn)
        {
            if (!CheckSearcher(_indexSearcher)) return null;
            return _indexSearcher.Search(new WildcardQuery(new Term("LogLevel", "*")), numberDocsToReturn);
        }

        public TopDocs GetAllExceptions(int numberDocsToReturn)
        {
            if (!CheckSearcher(_indexSearcher)) return null;
            return _indexSearcher.Search(_exceptionParser.Parse("Outer"), numberDocsToReturn);
        }

        private void InitializeIdList()
        {
            _monitorIdList = new HashSet<string>();
            _appNameList = new HashSet<string>();
            TopDocs hits = this.Search(new WildcardQuery(new Term("MonitorIdList", "*")));
            foreach (ScoreDoc doc in hits.ScoreDocs)
            {
                Document document = this.GetDocument(doc);
                string[] monitorIds = document.Get("MonitorIdList").Split(' ');
                foreach (string id in monitorIds)
                {
                    if (!_monitorIdList.Contains(id)) _monitorIdList.Add(id);
                }
                string[] appIds = document.Get("AppIdList").Split(' ');
                foreach (string id in appIds)
                {
                    if (!_appNameList.Contains(id)) _appNameList.Add(id);
                }
            }
        }

        private bool CheckSearcher(IndexSearcher searcher)
        {
            if (searcher == null)
                return false;
            return true;
        }
    }
}
