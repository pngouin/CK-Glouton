using CK.Glouton.Model.Server.Sender;

namespace CK.Glouton.Server.Senders
{
    public class MailSenderConfiguration : IAlertSenderConfiguration
    {
        // Implementation of IMailConfiguration

        public string Name { get; set; }
        public string Email { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }

        public bool Validate()
        {
            return !( string.IsNullOrEmpty( Name ) ||
                    string.IsNullOrEmpty( Email ) ||
                    string.IsNullOrEmpty( SmtpUsername ) ||
                    string.IsNullOrEmpty( SmtpPassword ) ||
                    string.IsNullOrEmpty( SmtpAddress ) ||
                    SmtpPort <= 0 );
        }

        public bool Equals( MailSenderConfiguration configuration )
        {
            return Name == configuration.Name
                && Email == configuration.Email
                && SmtpUsername == configuration.SmtpUsername
                && SmtpPassword == configuration.SmtpPassword
                && SmtpAddress == configuration.SmtpAddress
                && SmtpPort == configuration.SmtpPort;

        }

        // Implementation of IAlertSenderConfiguration

        public string SenderType { get; set; }

        public IAlertSenderConfiguration Clone()
        {
            return new MailSenderConfiguration
            {
                Name = Name,
                Email = Email,
                SmtpUsername = SmtpUsername,
                SmtpPassword = SmtpPassword,
                SmtpAddress = SmtpAddress,
                SmtpPort = SmtpPort
            };
        }
    }
}
