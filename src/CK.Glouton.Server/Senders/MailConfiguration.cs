using CK.Glouton.Model.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Server.Senders
{
    public class MailConfiguration : IMailConfiguration
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAdress { get; set; }
        public int SmptPort { get; set; }

        public bool Validate()
        {
            return ( string.IsNullOrEmpty(Name) ||
                    string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(SmtpUsername) ||
                    string.IsNullOrEmpty(SmtpPassword) ||
                    string.IsNullOrEmpty(SmtpAdress) ||
                    SmptPort <= 0);
        }
    }
}
