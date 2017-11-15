using CK.ControlChannel.Abstractions;
using CK.Core;
using System.IO;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;

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

        internal static ServerTestHelper DefaultServer
        (
            IAuthorizationHandler authorizationHandler = null,
            RemoteCertificateValidationCallback userCertificateValidationCallback = null
        )
        {
            return new ServerTestHelper
            (
                DefaultHost,
                DefaultPort,
                authorizationHandler ?? DefaultAuthHandler,
                null, // Todo: Add test certificate
                userCertificateValidationCallback
            );
        }

        internal static void Setup()
        {
            if( !System.Console.IsOutputRedirected )
                System.Console.OutputEncoding = Encoding.UTF8;

            LogFile.RootLogPath = GetTestLogDirectory();

            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
        }

        internal static string GetTestLogDirectory()
        {
            var logPath = Path.Combine( GetProjectPath(), "Logs" );
            if( !Directory.Exists( logPath ) )
                Directory.CreateDirectory( logPath );
            return logPath;
        }

        private static string GetProjectPath( [CallerFilePath]string path = null ) => Path.GetDirectoryName( path );
    }
}