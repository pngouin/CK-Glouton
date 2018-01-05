using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Lucene
{
    public class LuceneStatistics
    {
        LuceneSearcherManager _luceneSearcher;
        LuceneSearcherConfiguration _allSearchLog;
        LuceneSearcherConfiguration _allSearchException;

        public LuceneStatistics ( LuceneConfiguration configuration )
        {
            _luceneSearcher = new LuceneSearcherManager(configuration);
            _allSearchLog = new LuceneSearcherConfiguration
            {
                MaxResult = int.MaxValue
            };
            _allSearchLog.SearchAll(Model.Logs.LuceneWantAll.Log);

            _allSearchException = new LuceneSearcherConfiguration
            {
                MaxResult = int.MaxValue
            };
            _allSearchException.SearchAll(Model.Logs.LuceneWantAll.Exception);
        }


        public int LogInAppNameCount (string appName)
        {
            if (!_luceneSearcher.AppName.Contains(appName)) return 0;
            return _luceneSearcher.GetSearcher(appName).Search(_allSearchLog).Count;
        }

        public int AllLogCount () => _luceneSearcher.GetSearcher(_luceneSearcher.AppName.ToArray()).Search(_allSearchLog).Count;

        public int AllExceptionCount () => _luceneSearcher.GetSearcher(_luceneSearcher.AppName.ToArray()).Search(_allSearchException).Count;

        public IEnumerable<string> GetAppName => _luceneSearcher.AppName;
        public int AppNameCount() => _luceneSearcher.AppName.Count;

    }
}
