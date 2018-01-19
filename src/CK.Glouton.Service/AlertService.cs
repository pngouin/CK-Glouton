using System;
using System.Linq;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Services;
using CK.Glouton.Service.Common;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {
        private readonly AlertSenderParser _alertSenderParser;

        public AlertService()
        {
            _alertSenderParser = new AlertSenderParser();
        }

        public bool AddAlert( IAlertExpressionModel alertExpression )
        {
            try
            {
                var condition = alertExpression.Expressions.Build();
                var senders = alertExpression.Senders.Select( _alertSenderParser.Parse );

                return true;
            }
            catch( Exception )
            {
                return false;
            }
        }






    }
}