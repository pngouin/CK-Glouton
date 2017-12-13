using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Reflection;
using System.Threading;

namespace CK.Glouton.Server
{
    public sealed class HandlersManager : IDisposable
    {
        private readonly DispatcherSink _sink;

        public CancellationToken DisposingToken => _sink.StopToken;

        public IHandlersManagerSink Sink => _sink;

        public HandlersManager( IActivityMonitor activityMonitor )
        {
            _sink = new DispatcherSink( activityMonitor );
        }

        private void ApplyConfiguration( HandlersManagerConfiguration configuration, bool waitForApplication = false )
        {
            if( configuration == null )
                throw new ArgumentNullException( nameof( configuration ) );

            if( !configuration.InternalClone )
            {
                configuration = (HandlersManagerConfiguration)configuration.Clone();
                configuration.InternalClone = true;
            }

            _sink.ApplyConfiguration( configuration, waitForApplication );
        }

        public void Handle( ReceivedData receivedData )
        {
            _sink.Handle( receivedData );
        }

        public static Func<IGloutonHandlerConfiguration, IGloutonHandler> CreateHandler = configuration =>
        {
            var name = configuration.GetType().GetTypeInfo().FullName;
            if( !name.EndsWith( "Configuration" ) )
                throw new Exception( $"Configuration handler type name must end with 'Configuration': {name}." );
            name = configuration.GetType().AssemblyQualifiedName.Replace( "Configuration,", "," );
            var type = Type.GetType( name, true );
            return (IGloutonHandler)Activator.CreateInstance( type, configuration );
        };

        public void Start( HandlersManagerConfiguration configuration )
        {
            _sink.Start();
            ApplyConfiguration( configuration, true );
        }

        #region IDisposable Support

        public bool IsDisposed => !_sink.IsRunning;

        public void Dispose( int millisecondsBeforeForceClose = Timeout.Infinite )
        {
            if( _sink.Stop() )
                _sink.Finalize( millisecondsBeforeForceClose );
        }

        public void Dispose()
        {
            Dispose( Timeout.Infinite );
        }

        #endregion
    }
}