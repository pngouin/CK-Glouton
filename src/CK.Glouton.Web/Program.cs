using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CK.Glouton.Web
{
    public static class Program
    {
        public static void Main( string[] args )
        {
            BuildWebHost( args ).Run();
        }

        private static IWebHost BuildWebHost( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                .UseKestrel()
                .UseContentRoot( Directory.GetCurrentDirectory() )
                .UseMonitoring()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
    }
}
