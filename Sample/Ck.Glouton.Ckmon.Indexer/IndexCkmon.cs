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

            foreach (var path in GetFiles())
            {
                string appName = "CKMON-" + Guid.NewGuid().ToString().Substring(0, 8);
                using (var indexer = new LuceneIndexer(configuration))
                using (LogReader reader = LogReader.Open(path))
                {
                    reader.MoveNext();
                    for (; ; )
                    {
                        indexer.IndexLog(reader.CurrentMulticast, appName);
                        if (!reader.MoveNext())
                            return;
                    }
                }
            }
        }

        private string[] GetFiles ()
        {
            return Directory.GetFiles("./ckmon", "*.ckmon", SearchOption.TopDirectoryOnly);
        }
    }
}
