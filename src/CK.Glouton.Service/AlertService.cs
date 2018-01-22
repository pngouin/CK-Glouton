using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CK.ControlChannel.Tcp;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Services;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        private readonly ControlChannelClient _controlChannelCLient;
        private readonly TcpControlChannelConfiguration _configuration;
        private readonly IFormatter _formatter;
        private readonly MemoryStream _memoryStream;

        public AlertService(TcpControlChannelConfiguration configuration)
        {
            _configuration = configuration;
            _controlChannelCLient = new ControlChannelClient(
                _configuration.Host,
                _configuration.Port,
                _configuration.BuildAuthData(),
                _configuration.IsSecure
                );

            _controlChannelCLient.OpenAsync();

            _memoryStream = new MemoryStream();
            _formatter = new BinaryFormatter();
        }

        public bool SendNewAlert( IAlertExpressionModel alertExpression )
        {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _memoryStream.Flush();

            _formatter.Serialize(_memoryStream, alertExpression);
            _controlChannelCLient.SendAsync("AddAlertSender", _memoryStream.ToArray()).GetAwaiter().GetResult();

            return true;
        }
    }
}