using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using CK.Glouton.Model.Lucene;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Service
{
    public class LuceneSearcherService : ILuceneSearcherService
    {
        private readonly LuceneConfiguration _configuration;
        private readonly LuceneSearcherManager _searcherManager;

        public LuceneSearcherService( IOptions<LuceneConfiguration> configuration )
        {
            _configuration = configuration.Value;
            _searcherManager = new LuceneSearcherManager( _configuration );
        }

        public LuceneSearcherService ( LuceneConfiguration configuration)
        {
            _configuration = configuration;
            _searcherManager = new LuceneSearcherManager(_configuration);
        }

        public List<ILogViewModel> Search( string query, params string[] appNames )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MaxResult = _configuration.MaxSearch,
                Fields = new[] { "LogLevel", "Exception" },
                Query = query,
            };

            if( query == "*" )
            {
                configuration.SearchAll( ELuceneWantAll.Log );
                return _searcherManager.GetSearcher( appNames ).Search( configuration );
            }

            configuration.ESearchMethod = ESearchMethod.FullText;
            return _searcherManager.GetSearcher( appNames ).Search( configuration );
        }

        public List<ILogViewModel> GetAll( params string[] appNames )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MaxResult = _configuration.MaxSearch,
                Fields = new[] { "LogLevel" },
            };

            configuration.SearchAll( ELuceneWantAll.Log );

            return _searcherManager.GetSearcher( appNames ).Search( configuration );
        }

        /// <summary>
        /// Return the selected log.
        /// </summary>
        /// <param name="monitorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="fields"></param>
        /// <param name="logLevel"></param>
        /// <param name="query"></param>
        /// <param name="appNames"></param>
        /// <param name="groupDepth"></param>
        /// <returns></returns>
        public List<ILogViewModel> GetLogWithFilters( string monitorId, DateTime start, DateTime end, string[] fields, string[] logLevel, string query, string[] appNames, int groupDepth = 0 )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MonitorId = monitorId,
                DateStart = start,
                DateEnd = end,
                Fields = fields,
                LogLevel = logLevel,
                Query = query,
                MaxResult = _configuration.MaxSearch,
                GroupDepth = groupDepth
            };
            if( configuration.Fields == null )
                configuration.Fields = new[] { "LogLevel" };

            var logs = _searcherManager.GetSearcher( appNames )?.Search( configuration ) ?? new List<ILogViewModel>();
            return groupDepth == 0 ? logs : logs.TakeWhileInclusive( l => l.LogType != ELogType.CloseGroup ).ToList();
        }

        public List<string> GetMonitorIdList()
        {
            return _searcherManager.GetSearcher( GetAppNameList().ToArray() ).GetAllMonitorId().ToList();
        }

        /// <summary>
        /// Return of the App Name indexed by Lucene.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppNameList()
        {
            return _searcherManager.AppName.ToList();
        }
    }
}
