using CK.Glouton.Model.Server.Sender;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CK.Glouton.AlertSender
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
            foreach( var existingSender in _senders )
                if( existingSender.Match( configuration ) )
                    return existingSender;

            var newSender = CreateSender( configuration );
            _senders.Add( newSender );
            return newSender;

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