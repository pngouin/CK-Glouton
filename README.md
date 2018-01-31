# CK-Glouton

Fifth semester's student project for INTECH IT school.

## Build requirements

- Windows
- Powershell
- [.NET Core SDK 2.0](https://www.microsoft.com/net/download/core) (with .NET Core 2.0)
- [Visual Studio 2017](https://www.visualstudio.com/) (any edition) with .NET framework build tools

## Build instructions

1. Clone the repository
1. In Powershell, run `CodeCakeBuilder/Bootstrap.ps1`
1. Run `CodeCakeBuilder/bin/Release/CodeCakeBuilder.exe`

## Usage

1. Launch `CK.Glouton.Sample.Server` which is a basic implementation of `CK.Glouton.Server`.
1. Configure your app to implement an Activity Monitor with the `CK.Glouton.Handler.Tcp`.
1. Now you can send some log to the sample server!

### If you want to see your log and configure some alert

1. Build the Angular app `npm run build` in `<ProjectFolder>/src/CK.Glouton.Web/app`.
1. Create a `appsetting.json` from the already here `appsetting.template.json` (the Lucene read location needs to be the same as the Lucene write location).
1. Launch `CK.Glouton.Web`
1. Go to your site!

## Configuration

### The TCP Handler

```csharp
System.Console.OutputEncoding = System.Text.Encoding.Unicode;

ActivityMonitor.AutoConfiguration += monitor => monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );

LogFile.RootLogPath = /* Path to your local logs save */

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
```

See `CK.Glouton.Sample.Client` to see more.

### The Glouton Server

```csharp
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
            new LuceneGloutonHandlerConfiguration
            {
                MaxSearch = 1000
            },
            new AlertHandlerConfiguration
            {
                DatabasePath = "C:/Glouton/Alerts" // The folder where alerts will be saved, consider this as a mocked database.
            }
        }
    } );
}
```
See `CK.Glouton.Sample.Server` to see more.

### The Web Application

```json 
"Lucene": {
    "MaxSearch": "1000",
    "Path": "C:/Glouton/Logs",
    "Directory": ""
  },
  "TcpControlChannel": {
    "Host": "localhost",
    "Port": 33698,
    "IsSecure": false
  },
  "Database": {
    "Path": "C:/Glouton/Alerts"
  },
  "Monitoring": {
    "LogPath": "Logs",
    "LogLevel": "Debug",
    "GrandOutput": {
      "Handlers": {
        "Console": true
      }
    }
```

| Configuration | Description |
| ------------ | -------------|
|Lucene.MaxSearch | The maximum number of logs returned by a single research with Lucene |
|Lucene.Path | The folder where Lucene files will be saved.
|Lucene.Directory | Custom subfolder for above.
|TcpControlChannel.Host | IpAddress of your CK.Glouton.Server. |
|TcpControlChannel.Port | Port of your CK.Glouton.Server. |
|TcpControlChannel.IsSecure| False by default we didn't implement a secure connection yet!|
|Database.Path| The folder where alerts will be saved, consider this as a mocked database.|
|Monitoring.LogPath| Where log are written in local |
|Monitoring.Loglevel | Minimum LogLevel |
|Monitoring.GrandOutput.Handler.Console | Active Log in the console |

## NuGet packages

| Feed             | Handler | Server | 
| ---------------- | ------ | ------ | 
| MyGet CI    |[![MyGet Badge](https://buildstats.info/myget/glouton-ci/CK.Glouton.Handler.Tcp)](https://www.myget.org/feed/glouton-ci/package/nuget/CK.Glouton.Handler.Tcp) | [![MyGet Badge](https://buildstats.info/myget/glouton-ci/CK.Glouton.Server)](https://www.myget.org/feed/glouton-ci/package/nuget/CK.Glouton.Server)
| MyGet preview    |[![MyGet Badge](https://buildstats.info/myget/glouton-preview/CK.Glouton.Handler.Tcp)](https://www.myget.org/feed/glouton-preview/package/nuget/CK.Glouton.Handler.Tcp) | [![MyGet Badge](https://buildstats.info/myget/glouton-preview/CK.Glouton.Handler.Tcp)](https://www.myget.org/feed/glouton-preview/package/nuget/CK.Glouton.Handler.Tcp)

## Build status

| Branch   | Visual Studio 2017 |
| -------- | ------- |
| `latest` | [![Build status](https://ci.appveyor.com/api/projects/status/wfsk213d8ecvri62?svg=true)](https://ci.appveyor.com/project/ZooPin/ck-glouton) |
| `develop`  | [![Build status](https://ci.appveyor.com/api/projects/status/wfsk213d8ecvri62/branch/develop?svg=true)](https://ci.appveyor.com/project/ZooPin/ck-glouton/branch/develop) |

## Contributing

Anyone and everyone is welcome to contribute. Please take a moment to
review the [guidelines for contributing](CONTRIBUTING.md).

## License

Assets in this repository are licensed with the MIT License. For more information, please see [LICENSE.md](LICENSE.md).

## Open-source licenses

This repository and its components use the following open-source projects:

- [ZooPin/CK-ControlChannel](https://github.com/ZooPin/CK-ControlChannel-Tcp/), licensed under the [MIT License](https://github.com/ZooPin/CK-ControlChannel-Tcp/blob/master/LICENSE.md)
- [ZooPin/CK-ControlChannel-Abstractions](https://github.com/ZooPin/CK-ControlChannel-Abstractions), licensed under the [MIT License](https://github.com/invenietis/CK-ControlChannel-Abstractions/blob/master/LICENSE.md)
- [invenietis/CK-Core](https://github.com/invenietis/CK-Core), licensed under the [GNU Lesser General Public License v3.0](https://github.com/invenietis/CK-Core/blob/master/LICENSE)
- [invenietis/CK-Text](https://github.com/invenietis/CK-Text), licensed under the [MIT License](https://github.com/invenietis/CK-Text/blob/master/LICENSE)
- [invenietis/CK-ActivityMonitor](https://github.com/invenietis/CK-ActivityMonitor), licensed under the [GNU Lesser General Public License v3.0](https://github.com/invenietis/CK-ActivityMonitor/blob/master/LICENSE)
- [invenietis/CK-Monitoring](https://github.com/Invenietis/CK-Monitoring), licensed under the [MIT License](https://github.com/invenietis/CK-Monitoring/blob/master/LICENSE)