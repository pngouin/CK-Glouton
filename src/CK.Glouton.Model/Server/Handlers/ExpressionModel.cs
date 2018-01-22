using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server.Handlers
{
    [Serializable]
    class ExpressionModel : IExpressionModel
    {
        public string Field { get; set; }
        public string Operation { get; set; }
        public string Body { get; set; }
    }
}
