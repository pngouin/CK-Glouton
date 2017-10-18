using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Monitoring;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CK.Glouton.Server
{
    public class GloutonServer : IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;

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
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.Position = 0;
            _memoryStream.Write( data, 0, data.Length );

            var entry = LogEntry.Read( _binaryReader, version, out var badEndOfFile );

            OnGrandOutputEvent?.Invoke( this, new LogEntryEventArgs( entry, clientSession ) );

        }

        public void Open()
        {
            _controlChannelServer.Open();
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
