using System.Diagnostics;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Loggers;
using Microsoft.Diagnostics.Runtime;

namespace BenchmarkDotNet.Diagnostics.Windows
{
    public class ClrMdDiagnoser
    {
        protected Process process { get; set; }

        protected string FullTypeName { get; set; }

        protected string FullMethodName { get; set; }

        internal LogCapture Logger = new LogCapture();

        protected ClrRuntime SetupClrRuntime(DataTarget dataTarget)
        {
            var version = dataTarget.ClrVersions.Single();
            Logger?.WriteLine($"\nCLR Version: {version.Version} ({version.Flavor}), Dac: {version.DacInfo}");
            return version.CreateRuntime(); // this method takes care for dac stuff
        }

        protected void PrintRuntimeDiagnosticInfo(DataTarget dataTarget, ClrRuntime runtime)
        {
            Logger.WriteLine(LogKind.Header, "\nRuntime Diagnostic Information");
            Logger.WriteLine(LogKind.Header, "------------------------------");

            Logger.WriteLine(LogKind.Header, "\nDataTarget Info:");
            Logger.WriteLine(LogKind.Info, string.Format("  ClrVersion{0}: {1}", dataTarget.ClrVersions.Count > 1 ? "s" : "", string.Join(", ", dataTarget.ClrVersions)));
            Logger.WriteLine(LogKind.Info, "  Architecture: " + dataTarget.Architecture);
            Logger.WriteLine(LogKind.Info, string.Format("  PointerSize: {0} ({1}-bit)", dataTarget.PointerSize, dataTarget.PointerSize == 8 ? 64 : 32));
            Logger.WriteLine(LogKind.Info, "  SymbolPath: " + dataTarget.SymbolLocator.SymbolPath);

            Logger.WriteLine(LogKind.Header, "\nClrRuntime Info:");
            Logger.WriteLine(LogKind.Info, "  ServerGC: " + runtime.ServerGC);
            Logger.WriteLine(LogKind.Info, "  HeapCount: " + runtime.HeapCount);
            Logger.WriteLine(LogKind.Info, "  Thread Count: " + runtime.Threads.Count);

            Logger.WriteLine(LogKind.Header, "\nClrRuntime Modules:");
            foreach (var module in runtime.Modules)
            {
                Logger.WriteLine(LogKind.Info,
                    string.Format(
                        "  {0,36} Id:{1} - {2,10:N0} bytes @ 0x{3:X16}",
                        Path.GetFileName(module.FileName),
                        module.AssemblyId.ToString().PadRight(10),
                        module.Size,
                        module.ImageBase));
            }

            ClrHeap heap = runtime.GetHeap();
            Logger.WriteLine(LogKind.Header, "\nClrHeap Info:");
            Logger.WriteLine(LogKind.Info, $"  TotalHeapSize: {heap.TotalHeapSize:N0} bytes ({heap.TotalHeapSize / 1024.0 / 1024.0:N2} MB)");
            Logger.WriteLine(LogKind.Info, string.Format("  Gen0: {0,10:N0} bytes", heap.GetSizeByGen(0)));
            Logger.WriteLine(LogKind.Info, string.Format("  Gen1: {0,10:N0} bytes", heap.GetSizeByGen(1)));
            Logger.WriteLine(LogKind.Info, string.Format("  Gen2: {0,10:N0} bytes", heap.GetSizeByGen(2)));
            Logger.WriteLine(LogKind.Info, string.Format("   LOH: {0,10:N0} bytes", heap.GetSizeByGen(3)));

            Logger.WriteLine(LogKind.Info, "  Segments: " + heap.Segments.Count);
            foreach (var segment in heap.Segments)
            {
                Logger.WriteLine(LogKind.Info,
                    string.Format("    Segment: {0,10:N0} bytes, {1,10}, Gen0: {2,10:N0} bytes, Gen1: {3,10:N0} bytes, Gen2: {4,10:N0} bytes",
                        segment.Length,
                        segment.IsLarge ? "Large" : (segment.IsEphemeral ? "Ephemeral" : "Unknown"),
                        segment.Gen0Length,
                        segment.Gen1Length,
                        segment.Gen2Length));
            }

            Logger.WriteLine();
        }
    }
}
