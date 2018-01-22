using System;
using System.Collections.Generic;
using System.Text;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Server.Handlers
{
    [Serializable]
    public class AlertExpressionModel : IAlertExpressionModel
    {
        public IExpressionModel[] Expressions { get; set; }
        public IAlertSenderConfiguration[] Senders { get; set; }
    }
}
