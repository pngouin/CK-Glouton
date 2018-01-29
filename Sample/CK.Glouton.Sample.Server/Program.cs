using CK.Core;
using CK.Glouton.Common;
using CK.Glouton.Server;
using CK.Glouton.Server.Handlers;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Reflection;

namespace CK.Glouton.Sample.Server
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            SetupActivityMonitor();
            Run();
            GrandOutput.Default.Dispose();
        }

        private static void Run()
        {
            var activityMonitor = new ActivityMonitor();

            using( var server = new GloutonServer(
                "127.0.0.1",
                33698,
                activityMonitor,
                new SampleClientAuthorizationHandler()
            ) )
            {
                server.Open( new HandlersManagerConfiguration
                {
                    GloutonHandlers =
                    {
                        new BinaryGloutonHandlerConfiguration
                        {
                            Path = "Logs",
                            MaxCountPerFile = 10000,
                            UseGzipCompression = true
                        },
                        new LuceneGloutonHandlerConfiguration(),
                        new AlertHandlerConfiguration { DatabasePath = @"%localappdata%/Glouton/Alerts".GetPathWithSpecialFolders() }
                    }
                } );

                var doContinue = true;
                while( doContinue )
                {
                    System.Console.WriteLine( "Press q to quit" );
                    var readKey = System.Console.ReadKey( true );
                    switch( readKey.Key )
                    {
                        case ConsoleKey.Q:
                            activityMonitor.Info( "Goodbye" );
                            doContinue = false;
                            break;

                        default:
                            activityMonitor.Warn( $"Unknown key {readKey.Key}" );
                            break;
                    }
                }
            }
        }


        private static void SetupActivityMonitor()
        {
            System.Console.OutputEncoding = System.Text.Encoding.Unicode;
            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            LogFile.RootLogPath = GetLogDirectory();
            var grandOutputConfig = new GrandOutputConfiguration();
            grandOutputConfig.AddHandler( new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfig );
        }

        private static string GetLogDirectory()
        {
            return EnsureDirectory( Path.Combine( GetAssemblyDirectory(), "Logs" ) );
        }

        private static string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName( typeof( Program ).GetTypeInfo().Assembly.Location );
        }

        private static string EnsureDirectory( string path )
        {
            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
            return path;
        }
    }
}
