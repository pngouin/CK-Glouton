using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Lucene;
using CK.Monitoring;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CK.Glouton.Server
{
    public class GloutonServer : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _blockingQueue;
        private readonly ConcurrentQueue<Action> _processingQueue;
        private readonly Dictionary<string, LuceneIndexer> _indexerDictionary;

        private Task _blockingQueueThread;
        private Task _processingQueueThread;

        private bool _isDisposing;

        public GloutonServer(
            string boundIpAddress,
            int port,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null
        )
        {
            _controlChannelServer = new ControlChannelServer
            (
                boundIpAddress,
                port,
                clientAuthorizationHandler ?? new TcpAuthHandler(),
                serverCertificate,
                userCertificateValidationCallback
            );
            _controlChannelServer.RegisterChannelHandler( "GrandOutputEventInfo", HandleGrandOutputEventInfo );
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            _blockingQueue = new ConcurrentQueue<Action>();
            _processingQueue = new ConcurrentQueue<Action>();
            _indexerDictionary = new Dictionary<string, LuceneIndexer>();
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            _processingQueue.Enqueue( () => ProcessData( data, clientSession ) );
        }

        private void ProcessData( byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );
            clientSession.ClientData.TryGetValue( "AppName", out var appName );
            var clientData = clientSession.ClientData;


            if( _indexerDictionary.ContainsKey( appName ) )
            {
                _indexerDictionary.TryGetValue( appName, out var indexer );
                _blockingQueue.Enqueue( () => indexer.IndexLog( entry, clientData ) );
            }
            else
            {
                var indexer = new LuceneIndexer( appName );
                _indexerDictionary.Add( appName, indexer );
                _blockingQueue.Enqueue( () => indexer.IndexLog( entry, clientData ) );
            }
        }

        private void ProcessQueue( ConcurrentQueue<Action> queue )
        {
            while( !queue.IsEmpty || !_isDisposing )
            {
                queue.TryDequeue( out var action );
                action?.Invoke();
            }
        }

        private void DisposeIndexerByName( string name )
        {
            _indexerDictionary.TryGetValue( name, out var indexer );
            indexer.Dispose();
        }

        private void DisposeAllIndexer()
        {
            foreach( var entry in _indexerDictionary )
                entry.Value.Dispose();
        }

        public void Open()
        {
            _controlChannelServer.Open();
            _blockingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _blockingQueue ) );
            _processingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _processingQueue ) );
        }

        public void Close()
        {
            _controlChannelServer.Close();
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose( bool disposing )
        {
            if( _disposedValue )
                return;

            if( disposing )
            {
                _isDisposing = true;
                Close();
                _controlChannelServer.Dispose();
                System.Threading.SpinWait.SpinUntil( () => _blockingQueueThread.IsCompleted && _processingQueueThread.IsCompleted );
                _blockingQueueThread.Dispose();
                _processingQueueThread.Dispose();
                DisposeAllIndexer();
            }
            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose( true );
        }

        #endregion
    }
}
