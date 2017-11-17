using CK.ControlChannel.Abstractions;
using System;

namespace CK.Glouton.Tests
{
    public class TestAuthHandler : IAuthorizationHandler
    {
        private readonly Func<IServerClientSession, bool> _handler;

        public TestAuthHandler( Func<IServerClientSession, bool> handler )
        {
            _handler = handler;
        }

        public bool OnAuthorizeSession( IServerClientSession serverClientSession )
        {
            return _handler( serverClientSession );
        }
    }
}