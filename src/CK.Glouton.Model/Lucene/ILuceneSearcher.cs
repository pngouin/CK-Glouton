using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using CK.Glouton.Model.Logs;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcher
    {
        ISet<string> AppNameList { get; }
        ISet<string> MonitorIdList { get; }

        List<ILogViewModel> GetAllExceptions(int numberDocsToReturn);
        List<ILogViewModel> GetAllLog(int numberDocsToReturn);
        List<ILogViewModel> Search(Query searchQuery);
        List<ILogViewModel> Search(string searchQuery);

        Document GetDocument(ScoreDoc scoreDoc);
        Document GetDocument(Query query);
    }
}