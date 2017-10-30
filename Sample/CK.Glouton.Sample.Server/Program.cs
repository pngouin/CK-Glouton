using CK.Core;
using CK.Glouton.Server;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using System;
using System.IO;
using System.Reflection;
using CK.Glouton.Lucene;

namespace CK.Glouton.Sample.Server
{
    internal class Program
    {
        static LuceneIndexer indexer = new LuceneIndexer(LuceneConstant.GetPath()); //default path
        private static void Main( string[] args )
        {
            SetupActivityMonitor();
            var program = new Program();
            program.Run();
            GrandOutput.Default.Dispose();
            indexer.Dispose();
        }

        private void Run()
        {
            var activityMonitor = new ActivityMonitor();


            using ( var server = new GloutonServer(
                "127.0.0.1",
                33698,
                new SampleHandler()
            ) )
            {
                server.Open();

                server.OnGrandOutputEvent += Server_OnGrandOutputEvent;

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

        private static void Server_OnGrandOutputEvent( object sender, LogEntryEventArgs logEntryEventArgs )
        {
            IActivityMonitor activityMonitor = new ActivityMonitor();
            activityMonitor.Info( logEntryEventArgs.Entry.Text );
            indexer.IndexLog(logEntryEventArgs.Entry, 0);
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
