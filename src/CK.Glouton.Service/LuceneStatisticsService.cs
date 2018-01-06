using CK.Glouton.Lucene;
using CK.Glouton.Model.Lucene;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Service
{
    public class LuceneStatisticsService : ILuceneStatisticsService
    {
        private readonly LuceneStatistics _stats;

        public LuceneStatisticsService( IOptions<LuceneConfiguration> configuration )
        {
            _stats = new LuceneStatistics(configuration.Value);
        }

        public int AllLogCount() => _stats.AllLogCount;
        public int AllExceptionCount => _stats.AllExceptionCount;
        public int AppNameCount => _stats.AppNameCount;
        public IEnumerable<string> GetAppNames => _stats.GetAppNames;

        public Dictionary<string, int> GetLogByAppName()
        {
            return _stats.GetAppNames.ToDictionary(appName => appName, appName => _stats.LogInAppNameCount(appName));
        }

        public Dictionary<string, int> GetExceptionByAppName()
        {
            return _stats.GetAppNames.ToDictionary(appName => appName, appName => _stats.ExceptionInAppNameCount(appName));
        }

    }
}
