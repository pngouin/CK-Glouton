using System;

namespace CK.Glouton.Model.Lucene
{
    public interface ILuceneSearcherConfiguration
    {
        int MaxResult { get; set; }
        int? GroupDepth { get; set; }
        string MonitorId { get; set; }
        DateTime DateStart { get; set; }
        DateTime DateEnd { get; set; }
        string[] Fields { get; set; }
        string[] LogLevel { get; set; }
        string Query { get; set; }
        ESearchMethod ESearchMethod { get; set; }
        ILuceneSearcherConfiguration SearchAll( ELuceneWantAll wantAll );
    }
}
