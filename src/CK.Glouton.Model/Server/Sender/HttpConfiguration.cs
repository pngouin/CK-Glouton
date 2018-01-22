using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server.Sender
{
    [Serializable]
    public class HttpConfiguration : IHttpConfiguration
    {
        public string Url { get; set; }
    }
}
