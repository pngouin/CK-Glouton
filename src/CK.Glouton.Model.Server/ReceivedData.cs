using CK.ControlChannel.Abstractions;
using System.Collections.Generic;

namespace CK.Glouton.Model.Server
{
    public struct ReceivedData
    {
        public ReceivedData( IReadOnlyCollection<byte> data, IServerClientSession serverClientSession )
        {
            Data = data;
            ServerClientSession = serverClientSession;
        }

        public IReadOnlyCollection<byte> Data { get; }
        public IServerClientSession ServerClientSession { get; }
    }
}