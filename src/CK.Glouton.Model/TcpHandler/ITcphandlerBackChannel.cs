using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Glouton.Model.TcpHandler
{
    public interface ITcphandlerBackChannel : IDisposable
    {
        bool Activate(IActivityMonitor activityMonitor);
        void HandleBackChannel(IActivityMonitor monitor, byte[] data);
        void Deactivate(IActivityMonitor m);
    }
}
