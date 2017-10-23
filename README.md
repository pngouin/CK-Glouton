# CK-Glouton

TCP-based implementations of [CK.ControlChannel server and client](https://github.com/invenietis/CK-ControlChannel-Abstractions).

## Build requirements

- Windows
- Powershell
- [.NET Core SDK 2.0](https://www.microsoft.com/net/download/core) (with .NET Core 2.0)
- [Visual Studio 2017](https://www.visualstudio.com/) (any edition) with .NET framework build tools

## Build instructions

1. Clone the repository
1. In Powershell, run `CodeCakeBuilder/Bootstrap.ps1`
1. Run `CodeCakeBuilder/bin/Release/CodeCakeBuilder.exe`

## NuGet packages

| Feed             | Handler | Server |
| ---------------- | ------ | ------ |
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

- [invenietis/CK-ControlChannel](https://github.com/ZooPin/CK-ControlChannel-Tcp/), licensed under the [MIT License](https://github.com/ZooPin/CK-ControlChannel-Tcp/blob/master/LICENSE.md)
- [invenietis/CK-ControlChannel-Abstractions](https://github.com/invenietis/CK-ControlChannel-Abstractions), licensed under the [MIT License](https://github.com/invenietis/CK-ControlChannel-Abstractions/blob/master/LICENSE.md)
- [invenietis/CK-Core](https://github.com/invenietis/CK-Core), licensed under the [GNU Lesser General Public License v3.0](https://github.com/invenietis/CK-Core/blob/master/LICENSE)
- [invenietis/CK-Text](https://github.com/invenietis/CK-Text), licensed under the [MIT License](https://github.com/invenietis/CK-Text/blob/master/LICENSE)
- [invenietis/CK-ActivityMonitor](https://github.com/invenietis/CK-ActivityMonitor), licensed under the [GNU Lesser General Public License v3.0](https://github.com/invenietis/CK-ActivityMonitor/blob/master/LICENSE)
- [invenietis/CK-Monitoring](https://github.com/Invenietis/CK-Monitoring), licensed under the [MIT License](https://github.com/invenietis/CK-Monitoring/blob/master/LICENSE)