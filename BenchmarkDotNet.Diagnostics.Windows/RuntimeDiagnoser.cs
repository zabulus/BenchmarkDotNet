using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Reports;
using Microsoft.Diagnostics.Runtime;

namespace BenchmarkDotNet.Diagnostics.Windows
{
    public class RuntimeDiagnoser : ClrMdDiagnoser, IDiagnoser
    {
        private readonly List<OutputLine> output = new List<OutputLine>();

        public void Start(Benchmark benchmark)
        {
            // Do nothing
        }

        public void Stop(Benchmark benchmark, BenchmarkReport report)
        {
            // Do nothing
        }

        public void ProcessStarted(Process process)
        {
            // Do nothing
        }

        public void AfterBenchmarkHasRun(Benchmark benchmark, Process process)
        {
            var result = PrintDiagnostics(benchmark, process);
            output.AddRange(result);
        }

        public void ProcessStopped(Process process)
        {
            // Do nothing
        }

        public void DisplayResults(ILogger logger)
        {
            foreach (var line in output)
                logger.Write(line.Kind, line.Text);
        }

        private IList<OutputLine> PrintDiagnostics(Benchmark benchmark, Process process)
        {
            Logger.Clear();

            //Method name format: "BenchmarkDotNet.Samples.Infra.RunFast()" (NOTE: WITHOUT the return type)
            var methodInfo = benchmark.Target.Method;
            var fullTypeName = methodInfo.DeclaringType.FullName;

            var methodParams = string.Join(", ", methodInfo.GetParameters().Select(p => p.ParameterType.FullName));
            var fullMethodName = $"{fullTypeName}.{methodInfo.Name}({methodParams})";

            Logger.WriteLine($"\nPrinting Code for Method: {fullMethodName}");
            Logger.WriteLine($"Attaching to process {Path.GetFileName(process.MainModule.FileName)}, Pid={process.Id}");
            Logger.WriteLine($"Path {process.MainModule.FileName}");

            using (var dataTarget = DataTarget.AttachToProcess(process.Id, 5000, AttachFlag.NonInvasive))
            {
                var runtime = SetupClrRuntime(dataTarget);

                PrintRuntimeDiagnosticInfo(dataTarget, runtime);

                return Logger.CapturedOutput;
            }
        }
    }
}