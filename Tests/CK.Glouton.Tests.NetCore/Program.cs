using NUnitLite;
using System.Reflection;

namespace CK.Glouton.NetCore.Tests
{
    public class Program
    {
        public static int Main( string[] args )
        {
            return new AutoRun( Assembly.GetEntryAssembly() ).Execute( args );
        }
    }
}
