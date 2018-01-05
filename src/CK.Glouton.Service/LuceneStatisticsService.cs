using CK.Glouton.Lucene;
using CK.Glouton.Model.Lucene;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace CK.Glouton.Service
{
    public class LuceneStatisticsService : ILuceneStatisticsService
    {
        private readonly LuceneConfiguration _configuration;
        private readonly LuceneStatistics _stats;

        public LuceneStatisticsService( IOptions<LuceneConfiguration> configuration )
        {
            _configuration = configuration.Value;
            _stats = new LuceneStatistics( _configuration );
        }

        public int AllLogCount() => _stats.AllLogCount;
        public int AllExceptionCount => _stats.AllExceptionCount;
        public int AppNameCount => _stats.AppNameCount;
        public IEnumerable<string> GetAppNames => _stats.GetAppNames;

        public Dictionary<string, int> GetLogByAppName()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach( var appName in _stats.GetAppNames )
            {
                result.Add( appName, _stats.LogInAppNameCount( appName ) );
            }
            return result;
        }

        public Dictionary<string, int> GetExceptionByAppName()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach( var appName in _stats.GetAppNames )
            {
                result.Add( appName, _stats.ExceptionInAppNameCount( appName ) );
            }
            return result;
        }

    }
}
