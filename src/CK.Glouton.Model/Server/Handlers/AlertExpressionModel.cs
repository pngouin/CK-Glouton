using CK.Glouton.Model.Server.Sender;
using System;

namespace CK.Glouton.Model.Server.Handlers
{
    [Serializable]
    public class AlertExpressionModel
    {
        public ExpressionModel[] Expressions { get; set; }
        public AlertSenderConfiguration[] Senders { get; set; }
    }
}
