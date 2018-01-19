using CK.Glouton.Model.Server.Handlers;
using CK.Glouton.Model.Server.Sender;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Server.Senders
{
    public class MailSender : IAlertSender
    {
        private readonly MailboxAddress _from;
        private readonly List<MailboxAddress> _to = new List<MailboxAddress>();
        private readonly IMailConfiguration _configuration;

        public MailSender( IMailConfiguration configuration )
        {
            if( configuration.Validate() )
                throw new ArgumentException( nameof( configuration ) );
            _configuration = configuration;
            _from = new MailboxAddress( _configuration.Name, _configuration.Email );
        }

        public void AddReceiver( string name, string email )
        {
            _to.Add( new MailboxAddress( name, email ) );
        }

        public void Send( AlertEntry logEntry )
        {
            using( var client = new SmtpClient() )
            {
                client.Connect( _configuration.SmtpAdress, _configuration.SmptPort, SecureSocketOptions.Auto );
                client.Authenticate( _configuration.SmtpUsername, _configuration.SmtpPassword );
                client.Send( ConstructMail( logEntry ) );
                client.Disconnect( true );
            }
        }

        private MimeMessage ConstructMail( AlertEntry logEntry )
        {
            var message = new MimeMessage();
            message.From.Add( _from );
            foreach( var to in _to )
            {
                message.To.Add( to );
            }

            message.Subject = $"CK-Glouton Automatic Alert.";
            message.Body = new TextPart( "plain" )
            {
                Text = ConstructTextBody( logEntry )
            };

            return message;
        }

        private static string ConstructTextBody( AlertEntry logEntry )
        {
            var builder = new StringBuilder();
            builder.AppendLine( "Hi," );
            builder.AppendLine( $"File : {logEntry.FileName} : {logEntry.LineNumber}" );
            builder.AppendLine( $"LogLevel : {logEntry.LogLevel}" );
            builder.AppendLine( $"At time : {logEntry.LogTime}" );

            if( logEntry.Tags != null )
            {
                builder.AppendLine( $"Tags : {logEntry.Tags}" );
            }

            builder.AppendLine( $"Message : {logEntry.Text}" );

            if( logEntry.Conclusions != null )
            {
                builder.AppendLine( $"Conclusion : {logEntry.Conclusions}" );
            }

            if( logEntry.Exception != null )
            {
                builder.AppendLine( "Exception: " );
                logEntry.Exception.ToStringBuilder( builder, "" );
            }

            builder.AppendLine();
            builder.AppendLine( "Automatic message of CK-Glouton" );
            builder.AppendLine( "A problem ? https://github.com/ZooPin/ck-glouton/issues" );

            return builder.ToString();
        }
    }
}
