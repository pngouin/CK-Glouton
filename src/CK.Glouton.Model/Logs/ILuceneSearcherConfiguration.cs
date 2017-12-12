using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Logs
{
    public interface ILuceneSearcherConfiguration
    {
        string[] AppName { get; set; }
        uint MaxResult { get; set; }
        string MonitorId { get; set; }
        DateTime DateStart { get; set; }
        DateTime DateEnd { get; set; }
        string[] Fields { get; set; }
        string[] LogLevel { get; set; }
        string Query { get; set; }
        SearchMethod SearchMethod { get; set; }
        ILuceneSearcherConfiguration SearchAll(LuceneWantAll wantAll);

    }

    public enum LuceneWantAll
    {
        Log,
        Exception
    }

    public enum SearchMethod
    {
        WithConfigurationObject,
        FullText
    }
}
