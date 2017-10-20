using CK.ControlChannel.Abstractions;
using CK.Glouton.Server;
using System.IO;
using System.Net.Security;
using System.Reflection;

namespace CK.Glouton.Tests
{
    public class TestHelper
    {
        internal static string DefaultHost { get; } = "127.0.0.1";
        internal static ushort DefaultPort { get; } = 43712;

        private static IAuthorizationHandler _defaultAuthHandler;
        internal static IAuthorizationHandler DefaultAuthHandler
        {
            get { return _defaultAuthHandler ?? ( _defaultAuthHandler = new TestAuthHandler( s => true ) ); }
        }

        internal static GloutonServer DefaultServer
        (
            IAuthorizationHandler authorizationHandler = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null
        )
        {
            return new GloutonServer
            (
                DefaultHost,
                DefaultPort,
                authorizationHandler ?? DefaultAuthHandler,
                null, // Todo: Add test certificate
                userCertificateValidationCallback
            );
        }

        internal static string GetTestLogDirectory()
        {
            var dllPath = typeof( GloutonServerTests ).GetTypeInfo().Assembly.Location;
            var dllDir = Path.GetDirectoryName( dllPath );
            var logPath = Path.Combine( dllDir, "Logs" );
            if( !Directory.Exists( logPath ) )
                Directory.CreateDirectory( logPath );
            return logPath;
        }
    }
}