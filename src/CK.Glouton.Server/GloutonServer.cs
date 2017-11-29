using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Glouton.Server
{
    public class GloutonServer : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly IActivityMonitor _activityMonitor;
        private readonly ConcurrentQueue<Action> _processingQueue;
        private readonly List<IGloutonServerHandler> _handlers;


        private Task _processingQueueThread;

        public GloutonServer(
            string boundIpAddress,
            int port,
            IActivityMonitor activityMonitor = null,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null,
            params IGloutonServerHandler[] handlers
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
            _activityMonitor = activityMonitor;
            _processingQueue = new ConcurrentQueue<Action>();
            _handlers = new List<IGloutonServerHandler>();
            foreach( var handler in handlers )
                _handlers.Add( handler );

        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientServerSession )
        {
            _processingQueue.Enqueue( () => ProcessData( data, clientServerSession ) );
        }

        private void ProcessData( byte[] data, IServerClientSession serverClientSession )
        {
            foreach( var handler in _handlers )
                handler.OnGrandOutputEventInfo( data, serverClientSession );
        }

        public void Open()
        {
            _controlChannelServer.Open();
            _processingQueueThread = Task.Factory.StartNew( () => ProcessQueue( _processingQueue ) );
            foreach( var handler in _handlers )
                handler.Open( _activityMonitor );
        }

        private static void ProcessQueue( ConcurrentQueue<Action> concurrentQueue )
        {
            concurrentQueue.TryDequeue( out var action );
            action?.Invoke();
        }

        public void Close()
        {
            _controlChannelServer.Close();
            foreach( var handler in _handlers )
                handler.Close();
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _disposedValue )
                return;

            // Closing everything
            Close();

            SpinWait.SpinUntil( () => _processingQueueThread.IsCompleted );

            // Disposing everything
            _controlChannelServer.Dispose();
            _processingQueueThread.Dispose();

            foreach( var handler in _handlers )
                handler.Dispose();

            _disposedValue = true;
        }

        #endregion
    }
}
