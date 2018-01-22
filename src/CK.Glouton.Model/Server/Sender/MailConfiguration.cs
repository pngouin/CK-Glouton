using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server.Sender
{
    public class MailConfiguration : IMailConfiguration
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public string[] Contacts { get; set; }

        public bool Validate()
        {
            return !(string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(SmtpUsername) ||
                    string.IsNullOrEmpty(SmtpPassword) ||
                    string.IsNullOrEmpty(SmtpAddress) ||
                    SmtpPort <= 0);
        }
    }
}
