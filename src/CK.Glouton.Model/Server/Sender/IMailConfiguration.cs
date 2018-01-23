using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server.Sender
{
    public interface IMailConfiguration
    {
        string Name { get; set; }
        string Email { get; set; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string SmtpAddress { get; set; }
        int SmtpPort { get; set; }
        string[] Contacts { get; set; }
        bool Validate();
    }
}
