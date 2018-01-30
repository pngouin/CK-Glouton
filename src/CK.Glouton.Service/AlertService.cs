using CK.ControlChannel.Tcp;
using CK.Glouton.AlertSender.Sender;
using CK.Glouton.Common;
using CK.Glouton.Database;
using CK.Glouton.Model.Handler.Implementation;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Model.Web.Services;
using CK.Glouton.Model.Web.Services.Implementation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        private readonly ControlChannelClient _controlChannelClient;
        private readonly TcpControlChannelConfiguration _tcpConfiguration;
        private readonly DatabaseConfiguration _databaseConfiguration;
        private readonly IFormatter _formatter;
        private readonly MemoryStream _memoryStream;
        private readonly AlertTableMock _alertTableMock;

        public string[] AvailableConfiguration => new[] { "Mail", "Http" };

        public AlertService( IOptions<TcpControlChannelConfiguration> tcpConfiguration, IOptions<DatabaseConfiguration> databaseConfiguration )
        {
            _tcpConfiguration = tcpConfiguration.Value;
            _tcpConfiguration.AppName = typeof( AlertService ).Assembly.GetName().Name;
            _tcpConfiguration.PresentEnvironmentVariables = true;
            _tcpConfiguration.PresentMonitoringAssemblyInformation = true;
            _tcpConfiguration.HandleSystemActivityMonitorErrors = false;

            _controlChannelClient = new ControlChannelClient(
                _tcpConfiguration.Host,
                _tcpConfiguration.Port,
                _tcpConfiguration.BuildAuthData(),
                _tcpConfiguration.IsSecure
                );

            _controlChannelClient.OpenAsync().GetAwaiter().GetResult();

            _memoryStream = new MemoryStream();
            _formatter = new BinaryFormatter();

            _databaseConfiguration = databaseConfiguration.Value;
            _alertTableMock = new AlertTableMock( _databaseConfiguration.Path.GetPathWithSpecialFolders() );
        }

        public bool NewAlertRequest( AlertExpressionModel alertExpression )
        {
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            _memoryStream.Flush();

            foreach( var sender in alertExpression.Senders )
            {
                switch( sender.SenderType )
                {
                    case "Mail":
                        sender.Configuration = JObject.FromObject( sender.Configuration ).ToObject<MailSenderConfiguration>();
                        break;
                    case "Http":
                        sender.Configuration = JObject.FromObject( sender.Configuration ).ToObject<HttpSenderConfiguration>();
                        break;
                    default:
                        return false;
                }
            }

            _formatter.Serialize( _memoryStream, alertExpression );
            _controlChannelClient.SendAsync( "AddAlertSender", _memoryStream.ToArray() ).GetAwaiter().GetResult();

            return true;
        }

        private static MailSenderConfiguration _defaultMailSenderConfiguration;
        private static HttpSenderConfiguration _defaultHttpSenderConfiguration;
        public bool TryGetConfiguration( string key, out IAlertSenderConfiguration configuration )
        {
            configuration = null;
            switch( key )
            {
                case "Mail":
                    configuration = _defaultMailSenderConfiguration
                        ?? ( _defaultMailSenderConfiguration = (MailSenderConfiguration)new MailSenderConfiguration().Default() );
                    return true;

                case "Http":
                    configuration = _defaultHttpSenderConfiguration
                        ?? ( _defaultHttpSenderConfiguration = (HttpSenderConfiguration)new HttpSenderConfiguration().Default() );
                    return true;

                default:
                    return false;
            }
        }

        public IList<IAlertExpressionModel> GetAllAlerts()
        {
            return _alertTableMock.GetAll();
        }
    }
}