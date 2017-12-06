using System;
using System.Collections.Generic;
using System.IO;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Logs;
using Microsoft.Extensions.Options;

namespace CK.Glouton.Service
{
    public class LuceneSearcherService : ILuceneSearcherService
    {
        private readonly LuceneConfiguration _configuration;

        public LuceneSearcherService( IOptions<LuceneConfiguration> configuration )
        {
            _configuration = configuration.Value;
        }

        public List<ILogViewModel> Search( string directory, string query )
        {
            if( query == "*" )
                return GetAll( directory, 25 );

            var result = new List<ILogViewModel>();
            var luceneSearcher = new LuceneSearcher( _configuration, new[] { "LogLevel", "Exception" } );
            return luceneSearcher.Search( query );
        }

        public List<ILogViewModel> GetAll( string directory, int max )
        {
            var result = new List<ILogViewModel>();
            var searcher = new LuceneSearcher( _configuration, new[] { "LogLevel" } );
            return searcher.GetAllLog( max );
        }

        public List<ILogViewModel> GetLogWithFilters( string monitorId, string appName, DateTime start, DateTime end, string[] fields, string[] logLevel, string keyword )
        {
            var result = new List<ILogViewModel>();
            var searcher = new LuceneSearcher( _configuration, new[] { "LogLevel" } );
            return searcher.Search( searcher.CreateQuery( monitorId, appName, fields, logLevel, start, end, keyword ) );
        }

        public ISet<string> GetMonitorIdList()
        {
            return new LuceneSearcher( _configuration, new string[] { } ).MonitorIdList;
        }

        /// <summary>
        /// Return of the App Name indexed by Lucene.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetAppNameList()
        {
            var directoryInfo = new DirectoryInfo( _configuration.Path );
            var dirs = new HashSet<string>();

            foreach( var info in directoryInfo.GetDirectories() )
                dirs.Add( info.Name );

            return dirs;
        }
    }
}
