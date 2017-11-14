using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Monitoring;
using System;
using System.IO;
using System.Text;

namespace CK.Glouton.Handler.Tcp
{
    public class TcpHandler : IGrandOutputHandler
    {
        private readonly TcpHandlerConfiguration _configuration;
        private ControlChannelClient _controlChannelClient;
        private MemoryStream _memoryStream;
        private CKBinaryWriter _binaryWriter;

        public TcpHandler(TcpHandlerConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _memoryStream = new MemoryStream();
            _binaryWriter = new CKBinaryWriter(_memoryStream, Encoding.UTF8, true);
        }

        public bool Activate(IActivityMonitor activityMonitor)
        {
            if (_controlChannelClient == null)
                _controlChannelClient = new ControlChannelClient
                (
                    _configuration.Host,
                    _configuration.Port,
                    _configuration.BuildAuthData(),
                    _configuration.IsSecure,
                    _configuration.RemoteCertificateValidationCallback,
                    _configuration.LocalCertificateSelectionCallback,
                    _configuration.ConnectionRetryDelayMs
                );

            _controlChannelClient.OpenAsync(activityMonitor).GetAwaiter().GetResult();

            return true;
        }

        public bool ApplyConfiguration(IActivityMonitor activityMonitor, IHandlerConfiguration configuration)
        {
            return false;
        }

        public void Deactivate(IActivityMonitor m)
        {
            if (_controlChannelClient == null)
                return;

            _controlChannelClient.Dispose();
            _controlChannelClient = null;
            _memoryStream.Dispose();
            _binaryWriter.Dispose();
        }

        public void OnTimer(IActivityMonitor m, TimeSpan timerSpan)
        {
        }

        public void Handle(IActivityMonitor m, GrandOutputEventInfo logEvent)
        {
            _memoryStream.SetLength(0);
            _memoryStream.Seek(0, SeekOrigin.Begin);

            logEvent.Entry.WriteLogEntry(_binaryWriter);
            _controlChannelClient.SendAsync("GrandOutputEventInfo", _memoryStream.ToArray()).GetAwaiter().GetResult();
        }
    }
}