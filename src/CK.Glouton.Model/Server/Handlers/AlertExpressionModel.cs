using System;
using System.Collections.Generic;
using System.Text;
using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Model.Server.Handlers
{
    [Serializable]
    public class AlertExpressionModel 
    {
        public ExpressionModel[] Expressions { get; set; }
        public AlertSenderConfiguration[] Senders { get; set; }
    }
}
