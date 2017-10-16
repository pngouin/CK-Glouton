using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CK.Core;
using System.IO;
using CK.Monitoring;

namespace CK.Glouton.Server
{
    public class Server : IDisposable
    {
        private readonly ControlChannelServer _server;

        public event EventHandler<LogEntryEventArgs> OnGrandOutputEvent;
        public Server(
            string boundIpAddress,
            int port,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null
            )
        {
            _server = new ControlChannelServer(
                boundIpAddress,
                port,
                clientAuthorizationHandler ?? new TcpAuthHandler(),
                serverCertificate,
                userCertificateValidationCallback
                );
            _server.RegisterChannelHandler("GrandOutputEventInfo", HandleGrandOutputEventInfo);
        }

        private void HandleGrandOutputEventInfo(IActivityMonitor monitor, byte[] data, IServerClientSession clientSession)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (CKBinaryReader br = new CKBinaryReader(ms, Encoding.UTF8, true))
            {
                int version = Convert.ToInt32(clientSession.ClientData["LogEntryVersion"]);
                ILogEntry entry = LogEntry.Read(br, version, out bool badEof);

                OnGrandOutputEvent?.Invoke(this, new LogEntryEventArgs(entry, clientSession));
            }
        }
        public void Open()
        {
            _server.Open();
        }

        public void Close()
        {
            _server.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                    _server.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
