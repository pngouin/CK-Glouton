using System;
using System.Collections.Generic;
using CK.Monitoring;

namespace CK.Glouton.Lucene
{
    public interface IIndexer
    {
        void IndexLog(ILogEntry log, IReadOnlyDictionary<string, string> clientData);
    }
}
