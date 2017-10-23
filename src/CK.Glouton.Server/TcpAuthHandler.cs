using CK.ControlChannel.Abstractions;

namespace CK.Glouton.Server
{
    internal class TcpAuthHandler : IAuthorizationHandler
    {
        public bool OnAuthorizeSession( IServerClientSession s )
        {
            return true;
        }
    }
}