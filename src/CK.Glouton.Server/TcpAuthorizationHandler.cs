using CK.ControlChannel.Abstractions;

namespace CK.Glouton.Server
{
    internal class TcpAuthorizationHandler : IAuthorizationHandler
    {
        public bool OnAuthorizeSession( IServerClientSession s )
        {
            return true;
        }
    }
}