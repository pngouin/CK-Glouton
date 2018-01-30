using CK.Core;
using System;

namespace CK.Glouton.Model.Server.Handlers
{
    public interface IGloutonHandler : IDisposable
    {
        void Open( IActivityMonitor activityMonitor );
        void OnGrandOutputEventInfo( ReceivedData receivedData );
        void OnTimer( IActivityMonitor activityMonitor, TimeSpan timerDuration );
        bool ApplyConfiguration( IGloutonHandlerConfiguration configuration );
        void Close();
    }
}
