using System;
using System.Collections.Generic;

namespace CK.Glouton.Model
{
    public interface ILuceneSearcherService
    {
        List<ILogViewModel> Search( string query );
        List<ILogViewModel> GetAll( int max );
        List<ILogViewModel> GetLogWithFilters( string monitorId, string appName, DateTime start, DateTime end, string[] fields, string[] logLevel, string keyword );
        ISet<string> GetMonitorIdList();
        ISet<string> GetAppNameList();
    }
}
