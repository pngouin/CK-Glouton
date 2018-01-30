using CK.Monitoring;
using System.Collections.Generic;

namespace CK.Glouton.Lucene
{
    public interface IIndexer
    {
        void IndexLog( ILogEntry log, IReadOnlyDictionary<string, string> clientData );
    }
}
