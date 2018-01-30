using System;

namespace CK.Glouton.Model.Server.Sender.Implementation
{
    [Serializable]
    public class AlertSenderConfiguration : IAlertSenderConfiguration
    {
        public string SenderType { get; set; }
        public object Configuration { get; set; }

        public IAlertSenderConfiguration Default()
        {
            return new AlertSenderConfiguration
            {
                SenderType = "",
                Configuration = null
            };
        }

        public IAlertSenderConfiguration Clone()
        {
            return new AlertSenderConfiguration
            {
                SenderType = SenderType,
                Configuration = Configuration
            };
        }
    }
}
