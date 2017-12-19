using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Server.Handlers
{
    public class LuceneGloutonHandler : IGloutonHandler
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _blockingQueue;
        private readonly Dictionary<string, LuceneIndexer> _indexerDictionary;

        private LuceneGloutonHandlerConfiguration _configuration;

        private Task _blockingQueueThread;
        private bool _isDisposing;

        public LuceneGloutonHandler( LuceneGloutonHandlerConfiguration configuration )
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _blockingQueue = new ConcurrentQueue<Action>();
            _indexerDictionary = new Dictionary<string, LuceneIndexer>();
            _configuration = configuration;
        }

        /// <summary>
        /// Sends log into the queue to be indexed.
        /// </summary>
        /// <param name="receivedData"></param>
        public void OnGrandOutputEventInfo( ReceivedData receivedData )
        {
            var version = Convert.ToInt32( receivedData.ServerClientSession.ClientData[ "LogEntryVersion" ] as string );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( receivedData.Data.ToArray(), 0, receivedData.Data.Count );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );
            receivedData.ServerClientSession.ClientData.TryGetValue( "AppName", out var appName );
            var clientData = receivedData.ServerClientSession.ClientData as IReadOnlyDictionary<string, string>;

            if( !_indexerDictionary.TryGetValue( appName, out var indexer ) )
            {
                // Todo: Check for actual path related issues
                var luceneConfiguration = new LuceneConfiguration
                {
                    MaxSearch = _configuration.MaxSearch <= 0 ? 10 : _configuration.MaxSearch, // Todo: Improve the way which defines the default value
                    Path = _configuration.ActualPath,
                    OpenMode = _configuration.OpenMode,
                    Directory = appName
                };

                if( !Directory.Exists( luceneConfiguration.ActualPath ) )
                    Directory.CreateDirectory( luceneConfiguration.ActualPath );

                indexer = new LuceneIndexer( luceneConfiguration );
                _indexerDictionary.Add( appName, indexer );
            }

            _blockingQueue.Enqueue( () => indexer.IndexLog( entry, clientData ) );
        }

        public bool ApplyConfiguration( IGloutonHandlerConfiguration configuration )
        {
            return false;
        }

        private void DisposeAllIndexer()
        {
            foreach( var entry in _indexerDictionary )
                entry.Value.Dispose();
        }

        private void ProcessQueue( ConcurrentQueue<Action> queue )
        {
            while( !queue.IsEmpty || !_isDisposing )
            {
                if( queue.TryDequeue( out var action ) )
                    action?.Invoke();
            }
        }

        /// <summary>
        /// Starts queues.
        /// </summary>
        public void Open( IActivityMonitor activityMonitor )
        {
            _blockingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _blockingQueue ) );
        }

        /// <summary>
        /// Closes what need to be closed.
        /// </summary>
        public void Close()
        {
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _isDisposing || _disposedValue )
                return;

            _isDisposing = true;

            Close();
            System.Threading.SpinWait.SpinUntil( () => _blockingQueueThread.IsCompleted );

            _blockingQueueThread.Dispose();
            DisposeAllIndexer();

            _disposedValue = true;
        }

        #endregion
    }
}
