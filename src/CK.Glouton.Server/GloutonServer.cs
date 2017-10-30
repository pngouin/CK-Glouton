using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Monitoring;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CK.Glouton.Lucene;

namespace CK.Glouton.Server
{
    public class GloutonServer : IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private BlockingCollection<Action> _blockingQueue;
        private Dictionary<string, LuceneIndexer> _indexerDic;

        public event EventHandler<LogEntryEventArgs> OnGrandOutputEvent;

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
            _blockingQueue = new BlockingCollection<Action>();
            _indexerDic = new Dictionary<string, LuceneIndexer>();
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength(0);
            _memoryStream.Write(data, 0, data.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            var entry = LogEntry.Read( _binaryReader, version, out _ );

            //OnGrandOutputEvent?.Invoke( this, new LogEntryEventArgs( entry, clientSession ) );

            if (_indexerDic.ContainsKey(clientSession.ClientName))
            {
                LuceneIndexer indexer;
                _indexerDic.TryGetValue(clientSession.ClientName, out indexer);
                _blockingQueue.Add(() => indexer.IndexLog(entry, 0));
            }
            else
            {
                LuceneIndexer indexer = new LuceneIndexer(clientSession.ClientName);
                _indexerDic.Add(clientSession.ClientName, indexer);
                _blockingQueue.Add(() => indexer.IndexLog(entry, 0));
            }
        }

        private void ProcessQueue()
        {
            while (true)
            {
                foreach (Action action in _blockingQueue) action.Invoke();
            }
        }
        
        private void DisposeAllIndexer()
        {
            foreach (KeyValuePair<string, LuceneIndexer> entry in _indexerDic) entry.Value.Dispose();
        }

        public void Open()
        {
            _controlChannelServer.Open();
            Task task = Task.Factory.StartNew(() => ProcessQueue());
        }

        public void Close()
        {
            _controlChannelServer.Close();
        }

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose( bool disposing )
        {
            if( _disposedValue )
                return;

            if( disposing )
            {
                Close();
                _controlChannelServer.Dispose();
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
