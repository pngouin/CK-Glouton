using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using CK.Glouton.Model.Logs;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcher
    {
        ISet<string> GetAllMonitorID();
        Document GetDocument(Query query, int maxResult);
        Document GetDocument(ScoreDoc scoreDoc);
        Document GetDocument(string key, string value, int maxResult);
        TopDocs QuerySearch(Query query, int maxResult);
    }
}