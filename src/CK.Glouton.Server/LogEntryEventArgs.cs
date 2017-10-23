using CK.ControlChannel.Abstractions;
using CK.Monitoring;
using System;

namespace CK.Glouton.Server
{
    public class LogEntryEventArgs : EventArgs
    {
        public ILogEntry Entry { get; }
        public IServerClientSession Client { get; }

        public LogEntryEventArgs(ILogEntry entry, IServerClientSession client)
        {
            Entry = entry;
            Client = client;
        }
    }
}