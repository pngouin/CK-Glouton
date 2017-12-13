using CK.Glouton.Model.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public SearchMethod SearchMethod { get; set; }

        internal LuceneWantAll All { get; set; }
        internal bool WantAll;

        public ILuceneSearcherConfiguration SearchAll( LuceneWantAll all )
        {
            All = all;
            WantAll = true;
            Fields = new string[] { ( all == LuceneWantAll.Exception ) ? LogField.EXCEPTION : LogField.TEXT };
            return this;
        }
    }
}
