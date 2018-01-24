using CK.Glouton.Model.Server.Sender;
using System;

namespace CK.Glouton.AlertSender.Sender
{
    [Serializable]
    public class HttpSenderConfiguration : IAlertSenderConfiguration, IHttpSenderConfiguration
    {
        public string SenderType { get; set; } = "Http";

        public string Url { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            return new HttpSenderConfiguration
            {
                Url = Url,
                SenderType = SenderType
            };
        }

        private static IAlertSenderConfiguration _defaultConfiguration;

        public IAlertSenderConfiguration Default()
        {
            return _defaultConfiguration
                   ?? ( _defaultConfiguration = new HttpSenderConfiguration
                   {
                       Url = ""
                   } );
        }
    }
}