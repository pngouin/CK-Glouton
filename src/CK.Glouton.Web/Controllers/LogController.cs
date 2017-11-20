using CK.Glouton.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Web.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the controller for log related requests.
    /// </summary>
    [Route( "api/log" )]
    public class LogController : Controller
    {
        private readonly ILuceneSearcherService _luceneSearcherService;

        public LogController( ILuceneSearcherService luceneSearcherService )
        {
            _luceneSearcherService = luceneSearcherService;
        }

        /// <summary>
        /// Returns <paramref name="max"/> logs. <paramref name="max"/> is 500 by default.
        /// Match the following: <code>api/log?max=[max] -- GET</code>.
        /// </summary>
        /// <param name="max">The number of logs to return.</param>
        /// <returns></returns>
        [HttpGet]
        public List<ILogViewModel> GetAll( int max = 0 )
        {
            return _luceneSearcherService.GetAll( max == 0 ? max : 500 );
        }


        /// <summary>
        /// Returns logs matching <paramref name="query"/>. Lucene is doing the processing.
        /// Match the following: <code>api/log/search?query=[query] -- GET</code>.
        /// </summary>
        /// <param name="query">The query which will be processed by lucene.</param>
        /// <returns></returns>
        [HttpGet( "search" )]
        public List<ILogViewModel> Search( string query = "" )
        {
            return string.IsNullOrEmpty( query ) ? GetAll() : _luceneSearcherService.Search( query );
        }

        /// <summary>
        /// Returns logs matching given parameters. When not specified, a parameter will be treated as <value>"All"</value>
        /// Match the following: <code>api/log/search/date?from=[date]&to=[date]&monitor=[monitor]&fields=[fields]&keywords=[keywords] -- GET</code>.
        /// </summary>
        /// <param name="monitorId">The monitorId to get logs from. By default: <code>"All"</code>.</param>
        /// <param name="appName">The application name to get logs from. By default: <code>"All"</code>.</param>
        /// <param name="from">The lower date for the time span range..</param>
        /// <param name="to">The superior date for the time span range.</param>
        /// <param name="fields">Fields to look for. By default: <code>{ "Tags", "FileName", "Text" }</code>.</param>
        /// <param name="keyword">Which keywords to consider. By default: <code>"*"</code>.</param>
        /// <param name="logLevel">Log levels to consider during the search. By default: <code>{ "Debug", "Trace", "Info", "Warn", "Error", "Fatal" }</code>.</param>
        /// <returns></returns>
        [HttpGet( "search" )]
        public List<ILogViewModel> Search
        (
            string monitorId, string appName, DateTime from, DateTime to,
            string[] fields, string keyword, [FromQuery]string[] logLevel
        )
        {
            if( monitorId == null || monitorId == "*" )
                monitorId = "All";
            if( appName == null || appName == "*" )
                appName = "All";
            if( fields.Length == 0 || fields[ 0 ] == "*" )
                fields = new[] { "Tags", "FileName", "Text" };
            if( logLevel.Length == 0 || logLevel[ 0 ] == "*" )
                logLevel = new[] { "Debug", "Trace", "Info", "Warn", "Error", "Fatal" };
            if( keyword == null )
                keyword = "*";

            return _luceneSearcherService.GetLogWithFilters( monitorId, appName, from, to, fields, logLevel, keyword );
        }

        /// <summary>
        /// Returns the list of all monitors id.
        /// Match the following: <code>api/log/monitor?max=[max] -- GET</code>.
        /// </summary>
        /// <returns></returns>
        [HttpGet( "monitor" )]
        public ISet<string> GetAllMonitorId()
        {
            return _luceneSearcherService.GetMonitorIdList();
        }

        /// <summary>
        /// Returns the list of all application name.
        /// Match the following: <code>api/log/app?max=[max] -- GET</code>.
        /// </summary>
        /// <returns></returns>
        [HttpGet( "app" )]
        public ISet<string> GetAllAppName()
        {
            return _luceneSearcherService.GetAppNameList();
        }
    }
}
