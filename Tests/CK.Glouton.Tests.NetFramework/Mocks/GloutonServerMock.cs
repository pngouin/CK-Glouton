using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.Model.Server;
using CK.Monitoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CK.Glouton.Tests
{
    /// <inheritdoc cref="IGloutonServer" />
    /// <summary>
    /// Represents a mock glouton server *with no handlers support*.
    /// Received logs will be put in a list.
    /// </summary>
    public class GloutonServerMock : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly MemoryStream _memoryStream;
        private readonly CKBinaryReader _binaryReader;

        /// <summary>
        /// The list of received logs.
        /// </summary>
        public List<ILogEntry> ListLog { get; }

        /// <summary>
        /// Initializes a new <see cref="GloutonServerMock"/>.
        /// </summary>
        /// <param name="boundIpAddress">Host address. You can use <see cref="TestHelper.DefaultHost"/>.</param>
        /// <param name="port">Port. You can use <see cref="TestHelper.DefaultPort"/>.</param>
        /// <param name="clientAuthorizationHandler">Authorization Handler. If none are set, <see cref="TestAuthHandler"/> will be used.</param>
        /// <param name="serverCertificate">Server certificate. Can be null.</param>
        /// <param name="userCertificateValidationCallback">User certification call back. Can be null.</param>
        public GloutonServerMock(
            string boundIpAddress,
            int port,
            IAuthorizationHandler clientAuthorizationHandler = null,
            X509Certificate2 serverCertificate = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null )
        {
            _controlChannelServer = new ControlChannelServer
            (
                boundIpAddress,
                port,
                clientAuthorizationHandler ?? new TestAuthHandler( _ => true ),
                serverCertificate,
                userCertificateValidationCallback
            );
            _controlChannelServer.RegisterChannelHandler( "GrandOutputEventInfo", HandleGrandOutputEventInfo );
            _memoryStream = new MemoryStream();
            _binaryReader = new CKBinaryReader( _memoryStream, Encoding.UTF8, true );
            ListLog = new List<ILogEntry>();
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            var version = Convert.ToInt32( clientSession.ClientData[ "LogEntryVersion" ] );

            _memoryStream.SetLength( 0 );
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var entry = LogEntry.Read( _binaryReader, version, out _ );
            ListLog.Add( entry );
        }

        /// <summary>
        /// Returns a log entry matching <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ILogEntry GetLogEntry( string text )
        {
            return ListLog.Single( e => e.Text == text );
        }

        /// <summary>
        /// Close the handled control channel.
        /// </summary>
        public void Close()
        {
            _controlChannelServer.Close();
        }

        /// <summary>
        /// Open the handled control channel.
        /// </summary>
        public void Open( IHandlersManagerConfiguration handlersManagerConfiguration = null )
        {
            _controlChannelServer.Open();
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="handlersManagerConfiguration"></param>
        public void ApplyConfiguration( IHandlersManagerConfiguration handlersManagerConfiguration = null )
        {
        }

        #region IDisposable Support

        /// <summary>
        /// Dispose the handler control channel.
        /// </summary>
        public void Dispose()
        {
            _controlChannelServer.Dispose();
        }

        #endregion

    }
}