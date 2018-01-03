using CK.Core;
using CK.Glouton.Lucene;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ck.Glouton.Ckmon.Indexer
{
    public class IndexCkmon
    {
        public IndexCkmon(LuceneConfiguration configuration, string pathCkmon)
        {
            using (var indexer = new LuceneIndexer(configuration))
            using (LogReader reader = LogReader.Open(pathCkmon))
            {
                reader.MoveNext();
                for ( ; ; )
                {
                    indexer.IndexLog(reader.CurrentMulticast, configuration.Directory);
                    if (!reader.MoveNext())
                        return;
                }
            }
        }
    }
}
