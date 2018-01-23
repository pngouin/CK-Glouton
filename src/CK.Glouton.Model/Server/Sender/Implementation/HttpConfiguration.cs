using System;

namespace CK.Glouton.Model.Server.Sender.Implementation
{
    [Serializable]
    public class HttpConfiguration : IHttpConfiguration
    {
        public string Url { get; set; }
    }
}
