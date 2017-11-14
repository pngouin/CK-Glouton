using CK.Core;
using CK.Glouton.Handler.Tcp;
using CK.Monitoring;
using CK.Monitoring.Handlers;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Text;

namespace CK.Glouton.Tests
{
    public class GrandOuputServerHelper
    {
        public static void SetupServer()
        {
            if (!System.Console.IsOutputRedirected)
                System.Console.OutputEncoding = Encoding.UTF8;

            LogFile.RootLogPath = TestHelper.GetTestLogDirectory();

            ActivityMonitor.DefaultFilter = LogFilter.Debug;
            ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient(new ActivityMonitorConsoleClient());
             
            var grandOutputConfigurationServer = new GrandOutputConfiguration();
            grandOutputConfigurationServer.AddHandler(new TextFileConfiguration
            {
                MaxCountPerFile = 10000,
                Path = "Text",
            });
            GrandOutput.EnsureActiveDefault(grandOutputConfigurationServer);
        }
        
        public static void TearDown()
        {
            GrandOutput.Default.Dispose();
        }
    }
}
