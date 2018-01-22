using CK.ControlChannel.Tcp;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Model.Services;
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

            _configuration.AppName = typeof(AlertService).GetType().Assembly.GetName().Name;
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

        public bool SendNewAlert(AlertExpressionModel alertExpression )
        {
            _memoryStream.Seek( 0, SeekOrigin.Begin );
            _memoryStream.Flush();
            
            foreach(var sender in alertExpression.Senders)
            {
                switch (sender.SenderType)
                {
                    case "Mail":
                        sender.Configuration = JObject.FromObject(sender.Configuration).ToObject<MailConfiguration>();
                        break;
                    case "Http":
                        sender.Configuration = JObject.FromObject(sender.Configuration).ToObject<HttpConfiguration>();
                        break;
                    default: 
                        return false;
                }
            }

            _formatter.Serialize( _memoryStream, alertExpression );
            _controlChannelClient.SendAsync( "AddAlertSender", _memoryStream.ToArray() ).GetAwaiter().GetResult();

            return true;
        }

        public IMailConfiguration GetMailConfiguration()
        {
            return new MailConfiguration
            {
                Name = "",
                Email = "",
                Contacts = new string[] { },
                SmtpAddress = "",
                SmtpPassword = "",
                SmtpUsername = "",
                SmtpPort = -1
            };
        }

        public IHttpConfiguration GetHttpConfiguration()
        {
            return new HttpConfiguration
            {
                Url = ""
            };
        }
    }
}