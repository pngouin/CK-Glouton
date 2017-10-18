using System.IO;
using System.Reflection;

namespace CK.Glouton.Tests
{
    public class TestHelper
    {
        internal static string GetTestLogDirectory()
        {
            var dllPath = typeof( ServerTests ).GetTypeInfo().Assembly.Location;
            var dllDir = Path.GetDirectoryName( dllPath );
            var logPath = Path.Combine( dllDir, "Logs" );
            if( !Directory.Exists( logPath ) )
                Directory.CreateDirectory( logPath );
            return logPath;
        }
    }
}