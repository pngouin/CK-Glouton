using CK.ControlChannel.Abstractions;
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

namespace CK.Glouton.Server
{
    public class GloutonIndexer : IGloutonServerHandler, IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _blockingQueue;
        private readonly ConcurrentQueue<Action> _processingQueue;
        private readonly Dictionary<string, LuceneIndexer> _indexerDictionary;

        private Task _blockingQueueThread;
        private Task _processingQueueThread;
        private bool _isDisposing;


        public GloutonIndexer()
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader(_memoryStream, Encoding.UTF8, true);
            _blockingQueue = new ConcurrentQueue<Action>();
            _processingQueue = new ConcurrentQueue<Action>();
            _indexerDictionary = new Dictionary<string, LuceneIndexer>();
        }

        /// <summary>
        /// Send log into the queue to be indexed.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="data"></param>
        /// <param name="clientSession"></param>
        public void OnGrandOutputEventInfo(IActivityMonitor monitor, byte[] data, IServerClientSession clientSession)
        {
            _processingQueue.Enqueue(() => ProcessData(data, clientSession));
        }

        /// <summary>
        /// Start the queues.
        /// </summary>
        public void Open()
        {
            _blockingQueueThread = Task.Factory.StartNew(() => ProcessQueue(_blockingQueue));
            _processingQueueThread = Task.Factory.StartNew(() => ProcessQueue(_processingQueue));
        }

        /// <summary>
        /// Close wath need to be close.
        /// </summary>
        public void Close()
        {
            // Nothing...
        }

        /// <summary>
        /// Read the data and index them.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clientSession"></param>
        private void ProcessData(byte[] data, IServerClientSession clientSession)
        {
            var version = Convert.ToInt32(clientSession.ClientData["LogEntryVersion"]);

            _memoryStream.SetLength(0);
            _memoryStream.Write(data, 0, data.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            var entry = LogEntry.Read(_binaryReader, version, out _);
            clientSession.ClientData.TryGetValue("AppName", out var appName);
            var clientData = clientSession.ClientData;


            if (_indexerDictionary.ContainsKey(appName))
            {
                _indexerDictionary.TryGetValue(appName, out var indexer);
                _blockingQueue.Enqueue(() => indexer.IndexLog(entry, clientData));
            }
            else
            {
                var indexer = new LuceneIndexer(appName);
                _indexerDictionary.Add(appName, indexer);
                _blockingQueue.Enqueue(() => indexer.IndexLog(entry, clientData));
            }
        }

        private void DisposeIndexerByName(string name)
        {
            _indexerDictionary.TryGetValue(name, out var indexer);
            indexer.Dispose();
        }

        private void DisposeAllIndexer()
        {
            foreach (var entry in _indexerDictionary)
                entry.Value.Dispose();
        }

        private void ProcessQueue(ConcurrentQueue<Action> queue)
        {
            while (!queue.IsEmpty || !_isDisposing)
            {
                queue.TryDequeue(out var action);
                action?.Invoke();
            }
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                _isDisposing = true;
                Close();
                System.Threading.SpinWait.SpinUntil(() => _blockingQueueThread.IsCompleted && _processingQueueThread.IsCompleted);
                _blockingQueueThread.Dispose();
                _processingQueueThread.Dispose();
                DisposeAllIndexer();
            }
            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
