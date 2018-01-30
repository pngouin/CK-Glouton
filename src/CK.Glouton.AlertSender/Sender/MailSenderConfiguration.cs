using CK.Glouton.Model.Server.Sender;
using System;

namespace CK.Glouton.AlertSender.Sender
{
    [Serializable]
    public class MailSenderConfiguration : IAlertSenderConfiguration, IMailSenderConfiguration
    {
        // Implementation of IMailSenderConfiguration

        public string Name { get; set; }
        public string Email { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public string[] Contacts { get; set; }
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

        private static IAlertSenderConfiguration _defaultConfiguration;
        public IAlertSenderConfiguration Default()
        {
            return _defaultConfiguration
                   ?? ( _defaultConfiguration = new MailSenderConfiguration
                   {
                       SenderType = "Mail",
                       Name = "",
                       Email = "",
                       Contacts = new string[] { },
                       SmtpAddress = "",
                       SmtpPassword = "",
                       SmtpUsername = "",
                       SmtpPort = -1
                   } );
        }
    }
}
