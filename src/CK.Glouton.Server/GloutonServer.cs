using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Lucene;
using CK.Glouton.Model.Server;
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
        private readonly GloutonIndexer _gloutonIndexer;
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
            _gloutonIndexer = new GloutonIndexer();
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            _gloutonIndexer.OnGrandOutputEventInfo(monitor, data, clientSession);
        }

        public void Open()
        {
            _controlChannelServer.Open();
            _gloutonIndexer.Open();
        }

        public void Close()
        {
            _controlChannelServer.Close();
            _gloutonIndexer.Close();
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
