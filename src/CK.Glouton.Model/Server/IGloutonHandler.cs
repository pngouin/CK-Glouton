using CK.Core;
using System;

namespace CK.Glouton.Model.Server
{
    public interface IGloutonHandler : IDisposable
    {
        void Open( IActivityMonitor activityMonitor );
        void OnGrandOutputEventInfo( ReceivedData receivedData );
        bool ApplyConfiguration( IGloutonHandlerConfiguration configuration );
        void Close();
    }
}
