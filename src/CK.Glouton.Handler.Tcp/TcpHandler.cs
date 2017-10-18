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

        public TcpHandler( TcpHandlerConfiguration configuration )
        {
            _configuration = configuration ?? throw new ArgumentNullException( nameof( configuration ) );
        }

        public bool Activate( IActivityMonitor activityMonitor )
        {
            if( _controlChannelClient == null )
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

            _controlChannelClient.OpenAsync( activityMonitor ).GetAwaiter().GetResult();

            return true;
        }

        public bool ApplyConfiguration( IActivityMonitor activityMonitor, IHandlerConfiguration configuration )
        {
            return false;
        }

        public void Deactivate( IActivityMonitor m )
        {
            if( _controlChannelClient == null )
                return;

            _controlChannelClient.Dispose();
            _controlChannelClient = null;
        }

        public void OnTimer( IActivityMonitor m, TimeSpan timerSpan )
        {
            throw new NotImplementedException();
        }

        public void Handle( IActivityMonitor m, GrandOutputEventInfo logEvent )
        {
            using( var memoryStream = new MemoryStream() )
            using( var binaryWriter = new CKBinaryWriter( memoryStream, Encoding.UTF8, true ) )
            {
                logEvent.Entry.WriteLogEntry( binaryWriter );
                _controlChannelClient.SendAsync( "GrandOutputEventInfo", memoryStream.ToArray() ).GetAwaiter().GetResult();
            }
        }
    }
}
