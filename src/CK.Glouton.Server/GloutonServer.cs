using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CK.Glouton.Server
{
    public class GloutonServer : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly List<IGloutonServerHandler> _handlers;

        public GloutonServer(
            string boundIpAddress,
            int port,
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

            _handlers = new List<IGloutonServerHandler>();
            foreach( var handler in handlers )
                _handlers.Add( handler );

        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            foreach( var handler in _handlers )
                handler.OnGrandOutputEventInfo( monitor, data, clientSession );
        }

        public void Open()
        {
            _controlChannelServer.Open();
            foreach( var handler in _handlers )
                handler.Open();
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

            // Disposing everything
            _controlChannelServer.Dispose();
            foreach( var handler in _handlers )
                handler.Dispose();

            _disposedValue = true;
        }

        #endregion
    }
}
