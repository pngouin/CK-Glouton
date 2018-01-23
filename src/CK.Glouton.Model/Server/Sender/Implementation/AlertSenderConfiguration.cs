using System;

namespace CK.Glouton.Model.Server.Sender.Implementation
{
    [Serializable]
    public class AlertSenderConfiguration : IAlertSenderConfiguration
    {
        public string SenderType { get; set; }
        public object Configuration { get; set; }

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
