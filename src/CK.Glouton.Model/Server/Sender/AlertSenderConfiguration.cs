using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server.Sender
{
    [Serializable]
    public class AlertSenderConfiguration : IAlertSenderConfiguration
    {
        public string SenderType { get; set; }
        public object Configuration { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            throw new NotImplementedException();
        }
    }
}
