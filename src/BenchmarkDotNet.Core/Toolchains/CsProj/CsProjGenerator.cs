﻿#if !UAP
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.DotNetCli;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Toolchains.CsProj
{
    [PublicAPI]
    public class CsProjGenerator : DotNetCliGenerator
    {
        public CsProjGenerator(
            string targetFrameworkMoniker, Func<Platform, string> platformProvider, string runtime = null)
            : base(new CsProjBuilder(targetFrameworkMoniker), targetFrameworkMoniker, null, platformProvider, null, runtime)
        {
        }

        protected override string GetBuildArtifactsDirectoryPath(Benchmark benchmark, string programName)
            => Path.Combine(Path.GetDirectoryName(benchmark.Target.Type.GetTypeInfo().Assembly.Location), programName);

        protected override string GetProjectFilePath(string binariesDirectoryPath)
            => Path.Combine(binariesDirectoryPath, "BenchmarkDotNet.Autogenerated.csproj");

        protected override string GetBinariesDirectoryPath(string buildArtifactsDirectoryPath)
            => Path.Combine(buildArtifactsDirectoryPath, "bin", DotNetCliBuilder.Configuration, TargetFrameworkMoniker);

        protected override void GenerateProject(Benchmark benchmark, ArtifactsPaths artifactsPaths, IResolver resolver, ILogger logger)
        {
            string template = ResourceHelper.CoreHelper.LoadTemplate("CsProj.txt");
            var projectFile = GetProjectFilePath(benchmark.Target.Type, logger);

            string platform = PlatformProvider(benchmark.Job.ResolveValue(EnvMode.PlatformCharacteristic, resolver));
            string content = SetPlatform(template, platform);
            content = SetCodeFileName(content, Path.GetFileName(artifactsPaths.ProgramCodePath));
            content = content.Replace("$CSPROJPATH$", projectFile.FullName);
            content = SetTargetFrameworkMoniker(content, TargetFrameworkMoniker);
            content = SetRuntimeIdentifier(content, platform);
            content = content.Replace("$PROGRAMNAME$", artifactsPaths.ProgramName);
            content = content.Replace("$RUNTIMESETTINGS$", GetRuntimeSettings(benchmark.Job.Env.Gc, resolver));
            content = content.Replace("$COPIEDSETTINGS$", GetSettingsThatNeedsToBeCopied(projectFile));

            File.WriteAllText(artifactsPaths.ProjectFilePath, content);
        }

        private string GetRuntimeSettings(GcMode gcMode, IResolver resolver)
        {
            if (!gcMode.HasChanges)
                return string.Empty;

            return new StringBuilder(80)
                .AppendLine("<PropertyGroup>")
                    .AppendLine($"<ServerGarbageCollection>{gcMode.ResolveValue(GcMode.ServerCharacteristic, resolver).ToLowerCase()}</ServerGarbageCollection>")
                    .AppendLine($"<ConcurrentGarbageCollection>{gcMode.ResolveValue(GcMode.ConcurrentCharacteristic, resolver).ToLowerCase()}</ConcurrentGarbageCollection>")
                    .AppendLine($"<RetainVMGarbageCollection>{gcMode.ResolveValue(GcMode.RetainVmCharacteristic, resolver).ToLowerCase()}</RetainVMGarbageCollection>")
                .AppendLine("</PropertyGroup>")
                .ToString();
        }

        private string SetRuntimeIdentifier(string content, string platform)
        {
            if (string.IsNullOrEmpty(Runtime))
            {
                return content.Replace("$RUNTIMEID$", string.Empty);
            }

            /*
             * C:\Program Files\dotnet\sdk\1.0.0-rc3-004530\Sdks\Microsoft.NET.Sdk\build\Microsoft.NET.RuntimeIdentifierInference.targets(49,5): error : RuntimeIdentifier must be set for .NETFramework executables. Consider RuntimeIdentifier=win7-x86 or RuntimeIdentifier=win7-x64. [C:\Projects\BenchmarkDotNet\C_B_Job-RNOJWY\BenchmarkDotNet.Autogenerated.csproj]
             */
            return content.Replace("$RUNTIMEID$", $"<RuntimeIdentifier>{Runtime}-{platform}</RuntimeIdentifier>");
        }

        // the host project might contain some custom settings that needs to be copied, sth like
        // <NetCoreAppImplicitPackageVersion>2.0.0-beta-001607-00</NetCoreAppImplicitPackageVersion>
	    // <RuntimeFrameworkVersion>2.0.0-beta-001607-00</RuntimeFrameworkVersion>
        private string GetSettingsThatNeedsToBeCopied(FileInfo projectFile)
        {
            var customSettings = new StringBuilder();
            using (var file = new StreamReader(File.OpenRead(projectFile.FullName)))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("NetCoreAppImplicitPackageVersion") || line.Contains("RuntimeFrameworkVersion"))
                    {
                        customSettings.Append(line);
                    }
                }
            }

            return customSettings.ToString();
        }

        private static FileInfo GetProjectFilePath(Type benchmarkTarget, ILogger logger)
        {
            if (!GetSolutionRootDirectory(out var solutionRootDirectory))
            {
                logger.WriteLineError(
                    $"Unable to find .sln file. Will use current directory {Directory.GetCurrentDirectory()} to search for project file. If you don't use .sln file on purpose it should not be a problem.");
                solutionRootDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            // important assumption! project's file name === output dll name
            var projectName = benchmarkTarget.GetTypeInfo().Assembly.GetName().Name;

            // I was afraid of using .GetFiles with some smart search pattern due to the fact that the method was designed for Windows
            // and now .NET is cross platform so who knows if the pattern would be supported for other OSes
            var possibleNames = new HashSet<string> { $"{projectName}.csproj", $"{projectName}.fsproj" };
            var projectFile = solutionRootDirectory
                .EnumerateFiles("*.*", SearchOption.AllDirectories)
                .FirstOrDefault(file => possibleNames.Contains(file.Name));

            if (projectFile == default(FileInfo))
            {
                throw new NotSupportedException(
                    $"Unable to find {projectName} in {solutionRootDirectory.FullName} and its subfolders. Most probably the name of output exe is different than the name of the .(c/f)sproj");
            }
            return projectFile;
        }
    }
}
#endif