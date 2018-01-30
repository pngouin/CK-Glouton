using System;
using System.Collections.Generic;
using CK.Monitoring;

namespace CK.Glouton.Lucene
{
    public class NullIndexer : IIndexer
    {
        public NullIndexer()
        {
        }
        public void IndexLog( ILogEntry entry, IReadOnlyDictionary<string, string> clientData )
        {
        }
    }
}
