using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Monitoring;

namespace CK.Glouton.Lucene
{
    class CompositeIndexer : IIndexer
    {
        private List<IIndexer> _indexers;

        public CompositeIndexer ()
        {
            _indexers = new List<IIndexer>();
        }
        public void IndexLog(ILogEntry entry, IReadOnlyDictionary<string, string> clientData)
        {
            foreach (IIndexer indexer in _indexers) indexer.IndexLog(entry, clientData);
        }

        public void Add (IIndexer indexer)
        {
            if(!_indexers.Contains(indexer)) _indexers.Add(indexer);
        }

        public bool Remove(IIndexer indexer)
        {
            if( _indexers.Contains( indexer ) )
            {
                _indexers.Remove( indexer );
                return true;
            }
            return false;
        }
    }
}
