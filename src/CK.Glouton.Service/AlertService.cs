using System;
using CK.ControlChannel.Tcp;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Services;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        private readonly ControlChannelClient _controlChannelCLient;
        private readonly TcpControlChannelConfiguration _configuration;

        public AlertService(TcpControlChannelConfiguration configuration)
        {
            _configuration = configuration;
            _controlChannelCLient = new ControlChannelClient(
                _configuration.Host,
                _configuration.Port,
                _configuration.BuildAuthData(),
                _configuration.IsSecure
                );
        }

        [Obsolete( "Method is deprecated" )]
        public bool SendNewAlert( IAlertExpressionModel alertExpression )
        {

            return false;
        }
    }
}