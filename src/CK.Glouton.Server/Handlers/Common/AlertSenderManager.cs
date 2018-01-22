using CK.Glouton.Model.Server.Sender;
using CK.Glouton.Server.Senders;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CK.Glouton.Server.Handlers.Common
{
    public class AlertSenderManager
    {
        private readonly IList<IAlertSender> _senders;

        public AlertSenderManager()
        {
            _senders = new List<IAlertSender>();
        }

        public IAlertSender Parse( IAlertSenderConfiguration configuration )
        {
            foreach( var sender in _senders )
                if( sender.Match( configuration ) )
                    return sender;

            if( configuration.SenderType == "Mail" )
            {
                if( !( configuration is MailSenderConfiguration mailSenderConfiguration ) )
                    throw new ArgumentException( nameof( configuration.SenderType ) );
                var sender = CreateSender( mailSenderConfiguration );
                _senders.Add( sender );
                return sender;
            }

            throw new ArgumentException( nameof( configuration ) );
        }

        public static Func<IAlertSenderConfiguration, IAlertSender> CreateSender = configuration =>
        {
            var name = configuration.GetType().GetTypeInfo().FullName;
            if( !name.EndsWith( "Configuration" ) )
                throw new Exception( $"Configuration sender type name must end with 'Configuration': {name}." );
            name = configuration.GetType().AssemblyQualifiedName.Replace( "Configuration,", "," );
            var type = Type.GetType( name, true );
            return (IAlertSender)Activator.CreateInstance( type, configuration );
        };
    }
}