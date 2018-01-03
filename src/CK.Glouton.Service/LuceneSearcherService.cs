using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
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

        public List<ILogViewModel> Search( string query, params string[] appNames )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MaxResult = _configuration.MaxSearch,
                Fields = new[] { "LogLevel", "Exception" },
            };

            if( query == "*" )
            {
                configuration.SearchAll( LuceneWantAll.Log );
                return _searcherManager.GetSearcher( appNames ).Search( configuration );
            }

            configuration.SearchMethod = SearchMethod.FullText;
            return _searcherManager.GetSearcher( appNames ).Search( configuration );
        }

        public List<ILogViewModel> GetAll( params string[] appNames )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MaxResult = _configuration.MaxSearch,
                Fields = new[] { "LogLevel" },
            };

            configuration.SearchAll( LuceneWantAll.Log );

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
        /// <returns></returns>
        public List<ILogViewModel> GetLogWithFilters( string monitorId, DateTime start, DateTime end, string[] fields, string[] logLevel, string query, params string[] appNames )
        {
            var configuration = new LuceneSearcherConfiguration
            {
                MonitorId = monitorId,
                DateStart = start,
                DateEnd = end,
                Fields = fields,
                LogLevel = logLevel,
                Query = query,
                MaxResult = _configuration.MaxSearch
            };
            if( configuration.Fields == null )
                configuration.Fields = new[] { "LogLevel" };
            return LogsPrettifier( _searcherManager.GetSearcher( appNames )?.Search( configuration )?.OrderBy( l => l.LogTime ).ToList() ?? new List<ILogViewModel>() );
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

        private List<ILogViewModel> LogsPrettifier( List<ILogViewModel> logs )
        {
            for( var index = 0 ; index < logs.Count ; index += 1 )
            {
                if( logs[ index ].LogType != ELogType.OpenGroup )
                    continue;
                BuildChildren( ref logs, index++ );
            }
            return logs;
        }

        private void BuildChildren( ref List<ILogViewModel> logs, int index )
        {
            if( !( logs[ index++ ] is OpenGroupViewModel parent ) )
                throw new InvalidOperationException( nameof( parent ) );

            var indexSnapshot = index;
            for( ; index < logs.Count ; index += 1 )
            {
                switch( logs[ index ].LogType )
                {
                    case ELogType.OpenGroup:
                        BuildChildren( ref logs, index );
                        break;

                    case ELogType.CloseGroup:
                        parent.GroupLogs = logs.RemoveAndGetRange( indexSnapshot, Math.Max( index - indexSnapshot + 1, logs.Count - indexSnapshot ) );
                        return;

                    default:
                        continue;
                }
            }
            parent.GroupLogs = logs.RemoveAndGetRange( indexSnapshot, Math.Max( index - indexSnapshot + 1, logs.Count - indexSnapshot ) );
        }
    }
}
