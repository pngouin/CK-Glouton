using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Server.Senders
{
    public class MailConfiguration : IMailConfiguration
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAdress { get; set; }
        public int SmtpPort { get; set; }

        public bool Validate()
        {
            return !( string.IsNullOrEmpty( Name ) ||
                    string.IsNullOrEmpty( Email ) ||
                    string.IsNullOrEmpty( SmtpUsername ) ||
                    string.IsNullOrEmpty( SmtpPassword ) ||
                    string.IsNullOrEmpty( SmtpAdress ) ||
                    SmtpPort <= 0 );
        }
    }
}
