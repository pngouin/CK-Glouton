using CK.Glouton.Model.Logs;
using CK.Glouton.Model.Lucene;
using System;

namespace CK.Glouton.Lucene
{
    public class LuceneSearcherConfiguration : ILuceneSearcherConfiguration
    {
        public string MonitorId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string[] Fields { get; set; }
        public string[] LogLevel { get; set; }
        public string Query { get; set; }
        public int MaxResult { get; set; }
        public ESearchMethod ESearchMethod { get; set; }
        internal ELuceneWantAll All { get; set; }
        public int? GroupDepth { get; set; }

        internal bool WantAll;

        public ILuceneSearcherConfiguration SearchAll( ELuceneWantAll all )
        {
            All = all;
            WantAll = true;
            Fields = new string[] { ( all == ELuceneWantAll.Exception ) ? LogField.EXCEPTION : LogField.TEXT };
            return this;
        }
    }
}
