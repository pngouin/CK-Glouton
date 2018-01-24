using CK.Glouton.Model.Server.Sender;
using System;

namespace CK.Glouton.AlertSender.Sender
{
    [Serializable]
    public class HttpSenderSenderConfiguration : IAlertSenderConfiguration, IHttpSenderConfiguration
    {
        public string SenderType { get; set; } = "Http";

        public string Url { get; set; }
        public object Configuration { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            return new HttpSenderSenderConfiguration
            {
                Url = Url,
                SenderType = SenderType,
                Configuration = Configuration
            };
        }

        private static IAlertSenderConfiguration _defaultConfiguration;

        public IAlertSenderConfiguration Default()
        {
            return _defaultConfiguration
                   ?? ( _defaultConfiguration = new HttpSenderSenderConfiguration
                   {
                       Url = ""
                   } );
        }
    }
}