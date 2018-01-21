using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.Collections.Generic;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcher
    {
        ISet<string> GetAllMonitorId();
        Document GetDocument( Query query, int maxResult );
        Document GetDocument( ScoreDoc scoreDoc );
        Document GetDocument( string key, string value, int maxResult );
    }
}