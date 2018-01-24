using CK.ControlChannel.Tcp;
using CK.Glouton.AlertSender.Sender;
using CK.Glouton.Model.Server.Handlers.Implementation;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Model.Services;
using CK.Glouton.Model.Services.Implementation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        private readonly ControlChannelClient _controlChannelClient;
        private readonly TcpControlChannelConfiguration _configuration;
        private readonly IFormatter _formatter;
        private readonly MemoryStream _memoryStream;


        public string[] AvailableConfiguration => new[] { "Mail", "Http" };

        public AlertService( IOptions<TcpControlChannelConfiguration> configuration )
        {
            _configuration = configuration.Value;
            _configuration.AppName = typeof( AlertService ).Assembly.GetName().Name;
            _configuration.PresentEnvironmentVariables = true;
            _configuration.PresentMonitoringAssemblyInformation = true;
            _configuration.HandleSystemActivityMonitorErrors = false;

            _controlChannelClient = new ControlChannelClient(
                _configuration.Host,
                _configuration.Port,
                _configuration.BuildAuthData(),
                _configuration.IsSecure
                );

            _controlChannelClient.OpenAsync().GetAwaiter().GetResult();

            _memoryStream = new MemoryStream();
            _formatter = new BinaryFormatter();
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
    }
}