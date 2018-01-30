using CK.ControlChannel.Abstractions;
using CK.ControlChannel.Tcp;
using CK.Core;
using CK.Glouton.AlertSender.Sender;
using CK.Glouton.Model.Server;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Model.Server.Sender.Implementation;
using CK.Glouton.Server.Handlers;
using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;

namespace CK.Glouton.Server
{
    public class GloutonServer : IGloutonServer, IDisposable
    {
        private readonly ControlChannelServer _controlChannelServer;
        private readonly IActivityMonitor _activityMonitor;
        private readonly HandlersManager _handlersManager;
        private readonly IFormatter _formatter;
        private readonly MemoryStream _memoryStream;

        private HandlersManagerConfiguration _handlersManagerConfiguration;

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
            _controlChannelServer.RegisterChannelHandler( "AddAlertSender", AddAlertSender );
            _activityMonitor = activityMonitor;
            _handlersManager = new HandlersManager( _activityMonitor );
            _memoryStream = new MemoryStream();
            _formatter = new BinaryFormatter();
        }

        private void AddAlertSender( IActivityMonitor monitor, byte[] data, IServerClientSession clientSession )
        {
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            _memoryStream.Flush();
            _memoryStream.Write( data, 0, data.Length );
            _memoryStream.Seek( 0, SeekOrigin.Begin );

            var alertExpressionModel = (AlertExpressionModel)_formatter.Deserialize( _memoryStream );

            foreach( var gloutonHandler in _handlersManagerConfiguration.GloutonHandlers )
            {
                if( !( gloutonHandler is AlertHandlerConfiguration alertHandlerConfiguration ) )
                    continue;

                alertHandlerConfiguration.Alerts.Add( new AlertExpression(
                    alertExpressionModel.Expressions,
                    alertExpressionModel.Senders.Select( ParseSenderConfiguration ).ToArray()
                ) );
                ApplyConfiguration( _handlersManagerConfiguration );
                return;
            }
        }

        private static IAlertSenderConfiguration ParseSenderConfiguration( AlertSenderConfiguration configuration )
        {
            switch( configuration.SenderType )
            {
                case "Mail":
                    return (MailSenderConfiguration)configuration.Configuration;

                case "Http":
                    return (HttpSenderConfiguration)configuration.Configuration;

                default:
                    throw new InvalidOperationException( configuration.SenderType );
            }
        }

        private void HandleGrandOutputEventInfo( IActivityMonitor monitor, byte[] data, IServerClientSession clientServerSession )
        {
            _handlersManager.Handle( new ReceivedData( data, clientServerSession ) );
        }

        public void Open( IHandlersManagerConfiguration handlersManagerConfiguration )
        {
            if( handlersManagerConfiguration == null )
                throw new ArgumentNullException( nameof( handlersManagerConfiguration ) );

            _handlersManagerConfiguration = handlersManagerConfiguration
                as HandlersManagerConfiguration
                ?? throw new ArgumentException( nameof( handlersManagerConfiguration ) );

            _controlChannelServer.Open();
            _handlersManager.Start( _handlersManagerConfiguration );
        }

        public void ApplyConfiguration( IHandlersManagerConfiguration handlersManagerConfiguration )
        {
            if( handlersManagerConfiguration == null )
                throw new ArgumentNullException( nameof( handlersManagerConfiguration ) );

            _handlersManagerConfiguration = handlersManagerConfiguration
                as HandlersManagerConfiguration
                ?? throw new ArgumentException( nameof( handlersManagerConfiguration ) );
            _handlersManager.ApplyConfiguration( _handlersManagerConfiguration );
        }

        public void Close()
        {
            _controlChannelServer.Close();
        }

        internal class AlertExpression : IAlertExpressionModel
        {
            public IExpressionModel[] Expressions { get; set; }
            public IAlertSenderConfiguration[] Senders { get; set; }

            public AlertExpression( IExpressionModel[] expressions, IAlertSenderConfiguration[] senders )
            {
                Expressions = expressions;
                Senders = senders;
            }
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
