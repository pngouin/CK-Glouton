using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using CK.Glouton.Model.Logs;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcher
    {
        Query CreateQuery(ILuceneSearcherConfiguration configuration);
        List<string> GetAllMonitorID();
        Document GetDocument(Query query, int maxResult);
        Document GetDocument(ScoreDoc scoreDoc);
        TopDocs QuerySearch(Query query, int maxResult);
    }
}