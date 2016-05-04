#if !CORE
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Xunit;

namespace BenchmarkDotNet.IntegrationTests
{
    public class RuntimeDiagnoserTest
    {
        [Fact]
        public void ItDisplaysRuntimeInformations()
        {
            var logger = new AccumulationLogger();
            BenchmarkRunner
                .Run<RuntimeDiagnoserTest>(
                    ManualConfig.CreateEmpty()
                        .With(Job.Default)
                        .With(logger)
                        .With(new RuntimeDiagnoser()));

            var logOutput = logger.GetLog();

            Assert.Contains("TotalHeapSize", logOutput);
            Assert.Contains("Gen0", logOutput);
            Assert.Contains("Segments", logOutput);
        }

        [Benchmark]
        public Dictionary<string, string> DictionaryEnumeration()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var item in dictionary)
            {
                ;
            }
            return dictionary;
        }
    }
}
#endif