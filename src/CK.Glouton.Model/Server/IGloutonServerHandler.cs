using CK.ControlChannel.Abstractions;
using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.Server
{
    public interface IGloutonServerHandler : IDisposable
    {
        void Open();
        void OnGrandOutputEventInfo(IActivityMonitor monitor, byte[] data, IServerClientSession clientSession);
        void Close();
    }
}
