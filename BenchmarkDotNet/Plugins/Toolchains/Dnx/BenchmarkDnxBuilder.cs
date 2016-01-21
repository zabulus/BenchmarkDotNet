﻿using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Plugins.Toolchains.Results;
#if DNX451
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Compilation.Caching;
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Internal;
using Microsoft.Extensions.PlatformAbstractions;
using System.Runtime.Versioning;
#endif

namespace BenchmarkDotNet.Plugins.Toolchains.Dnx
{
    internal class BenchmarkDnxBuilder : IBenchmarkBuilder
    {

#if DNX451
        public BenchmarkBuildResult Build(BenchmarkGenerateResult generateResult)
        {
            const string projectName = "BenchmarkDotNet.Autogenerated";
            const string configuration = "Release";
            const string aspect = "";

            // https://github.com/aspnet/dnx => this repo is a great source of knowledge
            // you should take a look at Microsoft.Dnx.Compilation.LibraryExporter.ExportProject()

            var projectJsonPath = Path.Combine(generateResult.DirectoryPath, BenchmarkDnxGenerator.ProjectFileName);
            var outputPath = Path.Combine(generateResult.DirectoryPath, $"{projectName}.dll");

            var projectJsonContent = File.ReadAllText(projectJsonPath);
            var project = ProjectUtilities.GetProject(projectJsonContent, projectName, projectJsonPath);
            var targetFramework = project.GetTargetFrameworks().Single().FrameworkName;

            var runtimeEnvironment = PlatformServices.Default.Runtime;
            var hostEnvironment = PlatformServices.Default.Application;
            var benchmarkApplicationEnvironment = new ApplicationEnvironment(project, targetFramework, configuration, hostEnvironment);
            var defaultLoadContext = PlatformServices.Default.AssemblyLoadContextAccessor.Default;
            var compilationEngineContext = new CompilationEngineContext(benchmarkApplicationEnvironment, runtimeEnvironment, defaultLoadContext, new CompilationCache());
            var compilationEngine = new CompilationEngine(compilationEngineContext);

            var libraryExporter = compilationEngine.CreateProjectExporter(project, targetFramework, configuration);
            var projectCompiler = compilationEngine.GetCompiler(Project.DefaultRuntimeCompiler, defaultLoadContext);
            var compilationProjectContext = ToCompilationContext(project, targetFramework, configuration, aspect);

            var projectDependenciesExport =
                new Lazy<LibraryExport>(() => libraryExporter.GetAllDependencies(project.Name, aspect));

            var metadataProjectReference = projectCompiler.CompileProject(
                compilationProjectContext,
                () => projectDependenciesExport.Value,
                () => CompositeResourceProvider.Default.GetResources(project));

            var diagnosticResult = metadataProjectReference.EmitAssembly(outputPath);

            Exception diagnosticException = diagnosticResult.Success
                ? null
                : new Exception(string.Join("\r\n", diagnosticResult.Diagnostics.Select(message => message.Message)));

            return new BenchmarkBuildResult(generateResult, diagnosticResult.Success, diagnosticException);
        }

        // copied from Microsoft.Dnx.Compilation.ProjectExtensions
        private static CompilationProjectContext ToCompilationContext(Project self, FrameworkName frameworkName, string configuration, string aspect)
        {
            return new CompilationProjectContext(
                new CompilationTarget(self.Name, frameworkName, configuration, aspect),
                self.ProjectDirectory,
                self.ProjectFilePath,
                self.Title,
                self.Description,
                self.Copyright,
                self.Version.ToString(),
                self.AssemblyFileVersion,
                self.EmbedInteropTypes,
                new CompilationFiles(
                    self.Files.PreprocessSourceFiles,
                    self.Files.SourceFiles),
                self.GetCompilerOptions(frameworkName, configuration));
        }
#else
        public BenchmarkBuildResult Build(BenchmarkGenerateResult generateResult)
        {
            throw new NotSupportedException("Use ClassicBuilder instead");
        }
#endif
    }
}