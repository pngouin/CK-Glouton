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
        private ConcurrentQueue<Action> _blockingQueue;
        private Task _queueThread;
        private Dictionary<string, LuceneIndexer> _indexerDic;
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
            _indexerDic = new Dictionary<string, LuceneIndexer>();
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength(0);
            _memoryStream.Write(data, 0, data.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            var entry = LogEntry.Read( _binaryReader, version, out _ );
            string appName;
            string machineName;
            clientSession.ClientData.TryGetValue("AppName", out appName);
            clientSession.ClientData.TryGetValue("ClientName", out machineName);
            string[] path = { appName, machineName, clientSession.ClientName };

            if (_indexerDic.ContainsKey(machineName + clientSession.ClientName))
            {
                LuceneIndexer indexer;
                _indexerDic.TryGetValue(machineName + clientSession.ClientName, out indexer);
                _blockingQueue.Enqueue(() => indexer.IndexLog(entry, appName));
            }
            else
            {
                LuceneIndexer indexer = new LuceneIndexer(path);
                _indexerDic.Add(machineName + clientSession.ClientName, indexer);
                _blockingQueue.Enqueue(() => indexer.IndexLog(entry, appName));
            }
        }

        private void ProcessQueue()
        {
            while (!_blockingQueue.IsEmpty && !_isDisposing)
            {
                _blockingQueue.TryDequeue( out _);
            }
        }

        private void DisposeIndexerByName (string name)
        {
            LuceneIndexer indexer;
            _indexerDic.TryGetValue(name, out indexer);
            indexer.Dispose();
        }
        
        private void DisposeAllIndexer()
        {
            foreach (KeyValuePair<string, LuceneIndexer> entry in _indexerDic) entry.Value.Dispose();
        }

        public void Open()
        {
            _controlChannelServer.Open();
            _queueThread = Task.Factory.StartNew(() => ProcessQueue());
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
                _isDisposing = true;
                Close();
                _controlChannelServer.Dispose();
                System.Threading.SpinWait.SpinUntil(()=> _queueThread.IsCompleted);
                _queueThread.Dispose();
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
