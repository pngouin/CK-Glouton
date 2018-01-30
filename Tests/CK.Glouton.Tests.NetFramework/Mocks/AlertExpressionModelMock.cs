using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using System.Collections.Generic;
using System.Linq;

namespace CK.Glouton.Tests
{
    internal class AlertExpressionModelMock : IAlertExpressionModel
    {
        public IExpressionModel[] Expressions { get; set; }
        public IAlertSenderConfiguration[] Senders { get; set; }

        public AlertExpressionModelMock( IEnumerable<string[]> expressions, IAlertSenderConfiguration[] senders )
        {
            Expressions = expressions
                .Select( expression => new ExpressionModel { Field = expression[ 0 ], Operation = expression[ 1 ], Body = expression[ 2 ] } )
                .ToArray();
            Senders = senders;
        }

        internal class ExpressionModel : IExpressionModel
        {
            public string Field { get; set; }
            public string Operation { get; set; }
            public string Body { get; set; }
        }
    }
}