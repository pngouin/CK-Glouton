using CK.ControlChannel.Abstractions;
using CK.Core;
using System;

namespace CK.Glouton.Model.Server
{
    public interface IGloutonHandler : IDisposable
    {
        void Open( IActivityMonitor activityMonitor );
        void OnGrandOutputEventInfo( byte[] data, IServerClientSession clientSession );
        void Close();
    }
}
