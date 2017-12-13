using CK.Monitoring;
using System.Collections.Generic;

namespace CK.Glouton.Lucene
{
    public class CompositeIndexer : IIndexer
    {
        private readonly List<IIndexer> _indexers;

        public CompositeIndexer()
        {
            _indexers = new List<IIndexer>();
        }

        public void IndexLog( ILogEntry entry, IReadOnlyDictionary<string, string> clientData )
        {
            foreach( var indexer in _indexers )
                indexer.IndexLog( entry, clientData );
        }

        public void Add( IIndexer indexer )
        {
            if( !_indexers.Contains( indexer ) )
                _indexers.Add( indexer );
        }

        public bool Remove( IIndexer indexer )
        {
            if( !_indexers.Contains( indexer ) )
                return false;

            _indexers.Remove( indexer );
            return true;
        }
    }
}
