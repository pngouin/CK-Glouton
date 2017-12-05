using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.Documents;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcher
    {
        ISet<string> AppNameList { get; }
        ISet<string> MonitorIdList { get; }

        TopDocs GetAllExceptions(int numberDocsToReturn);
        TopDocs GetAllLog(int numberDocsToReturn);
        TopDocs Search(Query searchQuery);
        TopDocs Search(string searchQuery);

        Document GetDocument(ScoreDoc scoreDoc); 
    }
}