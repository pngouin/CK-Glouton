namespace CK.Glouton.Model.Server.Sender
{
    public interface IMailConfiguration
    {
        string Name { get; set; }
        string Email { get; set; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string SmtpAdress { get; set; }
        int SmptPort { get; set; }

        bool Validate();
    }
}
