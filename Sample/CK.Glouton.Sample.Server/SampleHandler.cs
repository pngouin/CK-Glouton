using CK.ControlChannel.Abstractions;
using CK.Core;

namespace CK.Glouton.Sample.Server
{
    internal class SampleHandler : IAuthorizationHandler
    {
        public bool OnAuthorizeSession( IServerClientSession serverClientSession )
        {
            IActivityMonitor activityMonitor = new ActivityMonitor();
            foreach( var keyValuePair in serverClientSession.ClientData )
                activityMonitor.Info( $"{keyValuePair.Key}: {keyValuePair.Value}" );
            return true;
        }
    }
}