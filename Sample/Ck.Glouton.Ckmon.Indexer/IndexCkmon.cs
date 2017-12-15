using CK.Core;
using CK.Glouton.Lucene;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ck.Glouton.Ckmon.Indexer
{
    public class IndexCkmon
    {
        public IndexCkmon(LuceneConfiguration configuration)
        {
            string path = GetFiles().First();
            using (var indexer = new LuceneIndexer(configuration))
            using (LogReader reader = LogReader.Open(path))
            {
                reader.MoveNext();
                for (; ; )
                {
                    indexer.IndexLog(reader.CurrentMulticast, configuration.Directory);
                    if (!reader.MoveNext())
                        return;
                }
            }
        }

        private string[] GetFiles ()
        {
            return Directory.GetFiles("./ckmon", "*.ckmon", SearchOption.TopDirectoryOnly);
        }
    }
}
