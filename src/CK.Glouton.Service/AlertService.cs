using System;
using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Services;

namespace CK.Glouton.Service
{
    public class AlertService : IAlertService
    {

        public AlertService()
        {
        }

        [Obsolete( "Method is deprecated" )]
        public bool SendNewAlert( IAlertExpressionModel alertExpression )
        {

            return false;
        }
    }
}