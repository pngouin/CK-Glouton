using CK.Core;
using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Reflection;

namespace CK.Glouton.Sample.Client
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            SetupActivityMonitor();
            var m = new ActivityMonitor();

            var doContinue = true;
            while( doContinue )
            {
                System.Console.WriteLine( $"Press q to quit, m to send a line, s to send a CriticalError" );
                var k = System.Console.ReadKey( true );
                switch( k.Key )
                {
                    case ConsoleKey.Q:
                        m.Info( "Goodbye" );
                        doContinue = false;
                        break;
                    case ConsoleKey.M:
                        m.Info( $"Hello world - {DateTime.Now:R} - {Guid.NewGuid()}" );
                        break;
                    case ConsoleKey.S:
                        m.Error( $"CriticalError - {DateTime.Now:R}" );
                        break;
                    default:
                        m.Warn( $"Unknown key {k.Key}" );
                        break;

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
            grandOutputConfig.AddHandler( new TcpHandlerConfiguration
            {
                Host = "127.0.0.1",
                Port = 33698,
                IsSecure = false,
                AppName = typeof( Program ).GetTypeInfo().Assembly.GetName().Name,
                PresentEnvironmentVariables = true,
                PresentMonitoringAssemblyInformation = true,
                HandleSystemActivityMonitorErrors = true,
            } );
            GrandOutput.EnsureActiveDefault( grandOutputConfig );
        }

        private static string GetLogDirectory()
        {
            var dllPath = typeof( Program ).GetTypeInfo().Assembly.Location;
            var dllDir = Path.GetDirectoryName( dllPath );
            var logPath = Path.Combine( dllDir, "Logs" );
            if( !Directory.Exists( logPath ) )
                Directory.CreateDirectory( logPath );
            return logPath;
        }
    }
}
