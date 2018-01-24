using CK.Glouton.Model.Server.Sender;
using System;

namespace CK.Glouton.AlertSender.Sender
{
    [Serializable]
    public class HttpSenderConfiguration : IAlertSenderConfiguration
    {
        public string SenderType { get; set; } = "Http";

        public string Url { get; set; }
        public object Configuration { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            return new HttpSenderConfiguration {
                Url = Url,
                SenderType = SenderType,
                Configuration = Configuration
            };
        }
    }
}