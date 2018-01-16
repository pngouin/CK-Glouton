using CK.Glouton.Model.Server;
using CK.Monitoring;
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
        MailboxAddress _from;
        List<MailboxAddress> _to = new List<MailboxAddress>();
        IMailConfiguration _configuration;

        public MailSender( IMailConfiguration configuration )
        {
            if( !configuration.Validate() )
                throw new ArgumentException( nameof( configuration ) );
            _configuration = configuration;
            _from = new MailboxAddress( _configuration.Name, _configuration.Email );
        }

        public void AddReceiver( string name, string email )
        {
            _to.Add( new MailboxAddress( name, email ) );
        }

        public void Send( ILogEntry logEntry )
        {
            using( var client = new SmtpClient() )
            {
                client.Connect( _configuration.SmtpAdress, _configuration.SmptPort, SecureSocketOptions.Auto );
                client.Authenticate( _configuration.SmtpUsername, _configuration.SmtpPassword );
                client.Send( ConstructMail( logEntry ) );
                client.Disconnect( true );
            }
        }

        private MimeMessage ConstructMail( ILogEntry log )
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
                Text = ConstructTextBody( log )
            };

            return message;
        }

        private string ConstructTextBody( ILogEntry log )
        {
            var builder = new StringBuilder();
            builder.AppendLine( "Hi," );
            builder.AppendLine( $"File : {log.FileName} : {log.LineNumber}" );
            builder.AppendLine( $"LogLevel : {log.LogLevel}" );
            builder.AppendLine( $"At time : {log.LogTime}" );

            if( log.Tags != null )
            {
                builder.AppendLine( $"Tags : {log.Tags}" );
            }

            builder.AppendLine( $"Message : {log.Text}" );

            if( log.Conclusions != null )
            {
                builder.AppendLine( $"Conclusion : {log.Conclusions}" );
            }

            if( log.Exception != null )
            {
                builder.AppendLine( "Exception: " );
                log.Exception.ToStringBuilder( builder, "" );
            }

            builder.AppendLine();
            builder.AppendLine( "Automatic message of CK-Glouton" );
            builder.AppendLine( "A problem ? https://github.com/ZooPin/ck-glouton/issues" );

            return builder.ToString();
        }
    }
}
