using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CK.Glouton.Server
{
    public class GloutonServer : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly IActivityMonitor _activityMonitor;
        private readonly HandlersManager _handlersManager;

        public GloutonServer(
            string boundIpAddress,
            int port,
            IActivityMonitor activityMonitor,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null
        )
        {
            _controlChannelServer = new ControlChannelServer
            (
                boundIpAddress,
                port,
                clientAuthorizationHandler ?? new TcpAuthorizationHandler(),
                serverCertificate,
                userCertificateValidationCallback
            );
            _controlChannelServer.RegisterChannelHandler( "GrandOutputEventInfo", HandleGrandOutputEventInfo );
            _activityMonitor = activityMonitor;
            _handlersManager = new HandlersManager( _activityMonitor );
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientServerSession )
        {
            _handlersManager.Handle( new ReceivedData( data, clientServerSession ) );
        }

        public void Open( IHandlersManagerConfiguration handlersManagerConfiguration )
        {
            if( handlersManagerConfiguration == null )
                throw new ArgumentNullException( nameof( handlersManagerConfiguration ) );

            if( !( handlersManagerConfiguration is HandlersManagerConfiguration ) )
                throw new ArgumentException( nameof( handlersManagerConfiguration ) );

            _controlChannelServer.Open();
            _handlersManager.Start( (HandlersManagerConfiguration)handlersManagerConfiguration );
        }

        public void ApplyConfiguration( IHandlersManagerConfiguration handlersManagerConfiguration )
        {
            if( handlersManagerConfiguration == null )
                throw new ArgumentNullException( nameof( handlersManagerConfiguration ) );

            if( !( handlersManagerConfiguration is HandlersManagerConfiguration ) )
                throw new ArgumentException( nameof( handlersManagerConfiguration ) );

            _handlersManager.ApplyConfiguration( (HandlersManagerConfiguration)handlersManagerConfiguration );
        }

        public void Close()
        {
            _controlChannelServer.Close();
        }

        #region IDisposable Support

        private bool _disposedValue;

        public void Dispose()
        {
            if( _disposedValue )
                return;

            Close();

            _handlersManager.Dispose();
            _controlChannelServer.Dispose();


            _disposedValue = true;
        }

        #endregion
    }
}
