using CK.ControlChannel.Abstractions;
using CK.Core;
using System;

namespace CK.Glouton.Model.Server
{
    public interface IGloutonServerHandler : IDisposable
    {
        void Open( IActivityMonitor activityMonitor );
        void OnGrandOutputEventInfo( byte[] data, IServerClientSession clientSession );
        void Close();
    }
}
