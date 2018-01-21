using CK.Glouton.Model.Logs;
using System;
using System.Collections.Generic;

namespace CK.Glouton.Model.Services
{
    public interface ILuceneSearcherService
    {
        List<ILogViewModel> GetAll( params string[] appNames );
        List<string> GetAppNameList();
        List<ILogViewModel> GetLogWithFilters( string monitorId, DateTime start, DateTime end, string[] fields, string[] logLevel, string query, string[] appNames, int groupDepth = 0 );
        List<string> GetMonitorIdList();
        List<ILogViewModel> Search( string query, params string[] appNames );
    }
}
