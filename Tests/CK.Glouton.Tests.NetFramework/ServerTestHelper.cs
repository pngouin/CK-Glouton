using CK.Glouton.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.ControlChannel.Abstractions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CK.ControlChannel.Tcp;
using System.IO;
using CK.Core;
using CK.Monitoring;

namespace CK.Glouton.Tests
{
    public class ServerTestHelper : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;
        private readonly List<ILogEntry> _listLog;


        public ServerTestHelper(
            string boundIpAddress,
            int port,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null)
        {
            _controlChannelServer = new ControlChannelServer
            (
                boundIpAddress,
                port,
                clientAuthorizationHandler ?? new TcpAuthHandler(),
                serverCertificate,
                userCertificateValidationCallback
            );
            _controlChannelServer.RegisterChannelHandler("GrandOutputEventInfo", HandleGrandOutputEventInfo);
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader(_memoryStream, Encoding.UTF8, true);
            _listLog = new List<ILogEntry>();
        }

        private void HandleGrandOutputEventInfo(IActivityMonitor monitor, byte[] data, IServerClientSession clientSession)
        {
            var version = Convert.ToInt32(clientSession.ClientData["LogEntryVersion"]);

            _memoryStream.SetLength(0);
            _memoryStream.Write(data, 0, data.Length);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            var entry = LogEntry.Read(_binaryReader, version, out _);
            _listLog.Add(entry);
        }

        public ILogEntry GetLogEntry(string text)
        {
            return _listLog.Single(e => e.Text == text);
        }

        public void Close()
        {
            _controlChannelServer.Open();
        }

        public void Open()
        {
            _controlChannelServer.Close();
        }

        #region IDisposable Support

        public void Dispose()
        {
            _controlChannelServer.Dispose();
        }

        #endregion

    }
}

internal class TcpAuthHandler : IAuthorizationHandler
{
    public bool OnAuthorizeSession(IServerClientSession s)
    {
        return true;
    }
}