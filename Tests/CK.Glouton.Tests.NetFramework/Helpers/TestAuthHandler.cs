using CK.ControlChannel.Abstractions;
using System;

namespace CK.Glouton.Tests
{
    /// <summary>
    /// Represents a basic auth handler.
    /// </summary>
    public class TestAuthHandler : IAuthorizationHandler
    {
        private readonly Func<IServerClientSession, bool> _handler;

        /// <summary>
        /// Initializes a new Test AuthHandler.
        /// </summary>
        /// <param name="handler">
        /// A func used for resolving <see cref="OnAuthorizeSession"/>.
        /// For test, you can use <code>s => true</code>.
        /// </param>
        public TestAuthHandler( Func<IServerClientSession, bool> handler )
        {
            _handler = handler;
        }

        /// <summary>
        /// Resolve session and returns a boolean if the session is authorized.
        /// </summary>
        /// <param name="serverClientSession">The session used while resolving the Func given in the constructor.</param>
        /// <returns></returns>
        public bool OnAuthorizeSession( IServerClientSession serverClientSession )
        {
            return _handler( serverClientSession );
        }
    }
}