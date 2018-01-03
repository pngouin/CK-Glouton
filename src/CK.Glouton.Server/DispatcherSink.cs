using CK.Core;
using CK.Glouton.Model.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Glouton.Server
{
    internal class DispatcherSink : IHandlersManagerSink
    {
        private readonly BlockingCollection<ReceivedData> _queue;
        private readonly Task _task;
        private readonly List<IGloutonHandler> _gloutonHandlers;

        private readonly object _configurationTrigger;
        private readonly IActivityMonitor _activityMonitor;
        private readonly CancellationTokenSource _stopTokenSource;

        private HandlersManagerConfiguration[] _newConfigurations;

        private TimeSpan _timerDuration;
        private long _deltaTicks;
        private long _nextTicks;
        private long _nextExternalTicks;
        private long _deltaExternalTicks;
        private Action _externalOnTimer;

        private volatile int _stopFlag;
        private volatile bool _forceClose;

        public TimeSpan TimerDuration
        {
            get => _timerDuration;
            set
            {
                if( _timerDuration == value )
                    return;
                _timerDuration = value;
                _deltaTicks = value.Ticks;
            }
        }

        public DispatcherSink( IActivityMonitor activityMonitor )
        {
            _activityMonitor = activityMonitor;
            _queue = new BlockingCollection<ReceivedData>();
            _gloutonHandlers = new List<IGloutonHandler>();
            _task = new Task( Process, TaskCreationOptions.LongRunning );
            _configurationTrigger = new object();
            _stopTokenSource = new CancellationTokenSource();
        }

        private void Process()
        {
            _activityMonitor.SetTopic( GetType().FullName );

            var newConfigurations = _newConfigurations;
            while( newConfigurations == null )
            {
                Thread.Sleep( 0 );
                newConfigurations = _newConfigurations;
            }

            DoConfigure( _newConfigurations );

            while( !_queue.IsCompleted && !_forceClose )
            {
                var hasEvent = _queue.TryTake( out var receivedData, 10 );

                newConfigurations = _newConfigurations;
                if( newConfigurations.Length > 0 )
                    DoConfigure( newConfigurations );

                List<IGloutonHandler> faulty = null;

                if( hasEvent )
                {
                    foreach( var handler in _gloutonHandlers )
                    {
                        try
                        {
                            handler.OnGrandOutputEventInfo( receivedData );
                        }
                        catch( Exception exception )
                        {
                            var message = $"{handler.GetType().FullName}.Handle() crashed.";
                            ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                            _activityMonitor.Fatal( message, exception );
                            if( faulty == null )
                                faulty = new List<IGloutonHandler>();
                            faulty.Add( handler );
                        }
                    }
                }

                var now = DateTime.UtcNow.Ticks;
                if( now >= _nextTicks )
                {
                    foreach( var handler in _gloutonHandlers )
                    {
                        try
                        {
                            handler.OnTimer( _activityMonitor, _timerDuration );
                        }
                        catch( Exception exception )
                        {
                            var message = $"{handler.GetType().FullName}.OnTimer() crashed.";
                            ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                            _activityMonitor.Fatal( message, exception );
                            if( faulty == null )
                                faulty = new List<IGloutonHandler>();
                            faulty.Add( handler );
                        }
                    }
                    _nextTicks = now + _deltaTicks;
                    if( now >= _nextExternalTicks )
                    {
                        _externalOnTimer();
                        _nextExternalTicks = now + _deltaExternalTicks;
                    }
                }

                if( faulty != null )
                {
                    foreach( var handler in faulty )
                    {
                        SafeActivateOrDeactivate( handler, false );
                        _gloutonHandlers.Remove( handler );
                    }
                }
            }

            foreach( var handler in _gloutonHandlers )
                SafeActivateOrDeactivate( handler, false );
        }

        private void DoConfigure( HandlersManagerConfiguration[] newConfigurations )
        {
            Util.InterlockedSet( ref _newConfigurations, t => t.Skip( newConfigurations.Length ).ToArray() );
            var configuration = newConfigurations[ newConfigurations.Length - 1 ];
            TimerDuration = configuration.TimerDuration;
            var toKeep = new List<IGloutonHandler>();

            for( var iConfiguration = 0 ; iConfiguration < configuration.GloutonHandlers.Count ; ++iConfiguration )
            {
                for( var iHandler = 0 ; iHandler < _gloutonHandlers.Count ; ++iHandler )
                {
                    try
                    {
                        if( !_gloutonHandlers[ iHandler ].ApplyConfiguration( configuration.GloutonHandlers[ iConfiguration ] ) )
                            continue;

                        configuration.GloutonHandlers.RemoveAt( iConfiguration-- );
                        toKeep.Add( _gloutonHandlers[ iHandler ] );
                        _gloutonHandlers.RemoveAt( iHandler );
                        break;
                    }
                    catch( Exception exception )
                    {
                        var handler = _gloutonHandlers[ iHandler ];
                        var message = $"Existing {handler.GetType().FullName} crashed with the configuration {configuration.GloutonHandlers[ iConfiguration ].GetType().FullName}.";
                        ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                        _activityMonitor.Fatal( message, exception );
                        _gloutonHandlers.RemoveAt( iHandler-- );
                        SafeActivateOrDeactivate( handler, false );
                    }
                }
            }

            foreach( var handler in _gloutonHandlers )
                SafeActivateOrDeactivate( handler, false );
            _gloutonHandlers.Clear();
            _gloutonHandlers.AddRange( toKeep );

            foreach( var handlerConfiguration in configuration.GloutonHandlers )
            {
                try
                {
                    var handler = HandlersManager.CreateHandler( handlerConfiguration );
                    if( SafeActivateOrDeactivate( handler, true ) )
                        _gloutonHandlers.Add( handler );
                }
                catch( Exception exception )
                {
                    var message = $"While creating handler for {configuration.GetType().FullName}.";
                    ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                    _activityMonitor.Fatal( message, exception );
                }
            }

            lock( _configurationTrigger )
                Monitor.PulseAll( _configurationTrigger );
        }

        private bool SafeActivateOrDeactivate( IGloutonHandler handler, bool activate )
        {
            try
            {
                if( activate )
                    handler.Open( _activityMonitor );
                else
                    handler.Close();
            }
            catch( Exception exception )
            {
                var message = $"Handler {handler.GetType().FullName} crashed during {( activate ? "activation" : "de-activation" )}.";
                ActivityMonitor.CriticalErrorCollector.Add( exception, message );
                _activityMonitor.Fatal( message, exception );
                return false;
            }
            return true;
        }

        public void Start( TimeSpan timerDuration, TimeSpan externalTimerDuration, Action externalTimer )
        {
            _task.Start();
            _timerDuration = timerDuration;
            _deltaTicks = timerDuration.Ticks;
            _deltaExternalTicks = externalTimerDuration.Ticks;
            _externalOnTimer = externalTimer;
            var now = DateTime.UtcNow.Ticks;
            _nextTicks = now + _deltaTicks;
            _nextExternalTicks = now + _deltaExternalTicks;
        }

        public CancellationToken StopToken => _stopTokenSource.Token;

        public bool Stop()
        {
            if( Interlocked.Exchange( ref _stopFlag, 1 ) == 0 )
            {
                _stopTokenSource.Cancel();
                _queue.CompleteAdding();
                return true;
            }
            return false;
        }

        public void Finalize( int millisecondsBeforeForceClose )
        {
            if( !_task.Wait( millisecondsBeforeForceClose ) )
                _forceClose = true;

            _task.Wait();
            _queue.Dispose();
            _stopTokenSource.Dispose();

            foreach( var h in _gloutonHandlers )
                h.Dispose();
        }

        public bool IsRunning => _stopFlag == 0;

        public void Handle( ReceivedData receivedData ) => _queue.Add( receivedData );

        public void ApplyConfiguration( HandlersManagerConfiguration configuration, bool waitForApplication )
        {
            Debug.Assert( configuration.InternalClone );
            Util.InterlockedAdd( ref _newConfigurations, configuration );
            if( waitForApplication )
            {
                lock( _configurationTrigger )
                {
                    HandlersManagerConfiguration[] newConfigurations;
                    while( _stopFlag == 0
                           && ( newConfigurations = _newConfigurations ) != null
                           && newConfigurations.Contains( configuration ) )
                    {
                        Monitor.Wait( _configurationTrigger );
                    }
                }
            }
        }
    }
}