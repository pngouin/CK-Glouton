using CK.ControlChannel.Tcp;
using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using CK.ControlChannel.Abstractions;

namespace CK.Glouton.Server
{
    public interface IGloutonServer
    {
        void Open();
        void Close();
    }
}
