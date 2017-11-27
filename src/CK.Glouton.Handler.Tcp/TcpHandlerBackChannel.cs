using CK.Core;
using CK.Glouton.Model.TcpHandler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Glouton.Handler.Tcp
{
    public class TcpHandlerBackChannel : ITcphandlerBackChannel
    {
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly ConcurrentQueue<Action> _processingQueue;
        private readonly ConcurrentQueue<Action> _blockingQueue;

        private bool _isDisposing;
        private Task _processingQueueThread;
        private Task _blockingQueueThread;

        public TcpHandlerBackChannel()
        {
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader(_memoryStream, Encoding.UTF8, true);
            _processingQueue = new ConcurrentQueue<Action>();
            _blockingQueue = new ConcurrentQueue<Action>();
        }

        public bool Activate(IActivityMonitor activityMonitor)
        {
            _processingQueueThread = Task.Factory.StartNew(() => ProcessQueue(_processingQueue));
            _blockingQueueThread = Task.Factory.StartNew(() => ProcessQueue(_blockingQueue));

            return true;
        }

        private void ProcessData(byte[] data)
        {
            _memoryStream.SetLength(0);
            _memoryStream.Write(data, 0, data.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            _blockingQueue.Enqueue(() => ); // TODO: Do something
        }

        private void ProcessQueue(ConcurrentQueue<Action> queue)
        {
            while (!queue.IsEmpty || !_isDisposing)
            {
                queue.TryDequeue(out var action);
                action?.Invoke();
            }
        }

        public void Deactivate(IActivityMonitor m)
        {
            // Nothing
            
        }

        public void HandleBackChannel(IActivityMonitor monitor, byte[] data)
        {
            _processingQueue.Enqueue(() => ProcessData(data));
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
                SpinWait.SpinUntil(() => _blockingQueueThread.IsCompleted && _processingQueueThread.IsCompleted);
                _blockingQueueThread.Dispose();
                _processingQueueThread.Dispose();
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