using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Server.Senders
{
    public class HttpSenderConfiguration : IAlertSenderConfiguration
    {
        public string SenderType { get; set; } = "Http";

        public string Url { get; set; }
        public object Configuration { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            return new HttpSenderConfiguration { Url = Url };
        }
    }
}