**BenchmarkDotNet** is a powerful .NET library for benchmarking.

[![NuGet](https://img.shields.io/nuget/v/BenchmarkDotNet.svg)](https://www.nuget.org/packages/BenchmarkDotNet/) [![Gitter](https://img.shields.io/gitter/room/dotnet/BenchmarkDotNet.svg)](https://gitter.im/dotnet/BenchmarkDotNet) [![Build status](https://img.shields.io/appveyor/ci/perfdotnet/benchmarkdotnet/master.svg?label=appveyor)](https://ci.appveyor.com/project/perfdotnet/benchmarkdotnet/branch/master) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![Overview](https://img.shields.io/badge/docs-Overview-green.svg?style=flat)](http://benchmarkdotnet.org/Overview.htm) [![ChangeLog](https://img.shields.io/badge/docs-ChangeLog-green.svg?style=flat)](https://github.com/dotnet/BenchmarkDotNet/wiki/ChangeLog)

## Summary

* Standard benchmarking routine: generating an isolated project per each benchmark method; auto-selection of iteration amount; warmup; overhead evaluation; statistics calculation; and so on.
* Supported runtimes: Full .NET Framework, .NET Core (RTM), Mono
* Supported languages: C#, F#, and Visual Basic
* Supported OS: Windows, Linux, MacOS
* Easy way to compare different environments (`x86` vs `x64`, `LegacyJit` vs `RyuJit`, and so on; see: [Jobs](http://benchmarkdotnet.org/Configs/Jobs.htm))
* Reports: markdown, csv, html, plain text, png plots.
* Advanced features: [Baseline](http://benchmarkdotnet.org/Advanced/Baseline.htm), [Params](http://benchmarkdotnet.org/Advanced/Params.htm)
* Powerful diagnostics based on ETW events (see [BenchmarkDotNet.Diagnostics.Windows](https://www.nuget.org/packages/BenchmarkDotNet.Diagnostics.Windows/))

## Useful links

* [Documentation](http://benchmarkdotnet.org/)
  * [Overview](http://benchmarkdotnet.org/Overview.htm)
  * [Getting Started](http://benchmarkdotnet.org/GettingStarted.htm)
  * [Intro Samples](https://github.com/dotnet/BenchmarkDotNet/tree/master/samples/BenchmarkDotNet.Samples/Intro)
  * [Contributing](http://benchmarkdotnet.org/Contributing.htm)
  * [Team](http://benchmarkdotnet.org/Team.htm)
* NuGet Packages
  * [BenchmarkDotNet](https://www.nuget.org/packages/BenchmarkDotNet/)
  * [BenchmarkDotNet.Diagnostics.Windows](https://www.nuget.org/packages/BenchmarkDotNet.Diagnostics.Windows/)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/)
to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct). 

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).