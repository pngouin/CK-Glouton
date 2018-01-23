using System;

namespace CK.Glouton.Model.Server.Handlers.Implementation
{
    [Serializable]
    public class ExpressionModel : IExpressionModel
    {
        public string Field { get; set; }
        public string Operation { get; set; }
        public string Body { get; set; }
    }
}
