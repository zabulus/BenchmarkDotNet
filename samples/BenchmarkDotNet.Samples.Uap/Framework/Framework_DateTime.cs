using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.Uap;
using System.IO;

namespace BenchmarkDotNet.Samples.Framework
{
    [Config(typeof(Config))]
    public class Framework_DateTime
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                var infrastructure = new InfrastructureMode();
                infrastructure.Toolchain = new UapToolchain(new UapToolchainConfig()
                {
                    CSRFCookieValue = "2LYMHOZH9C4e1PUcBQVkkNDFikCXpY6v",
                    DevicePortalUri = "https://192.168.1.51/",
                    UAPBinariesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uap"),
                    WMIDCookieValue = "3914980555611581204597258868819265487056560409287773938948508430"
                });

                var uapJob = new Job("Uap", EnvMode.Uap, infrastructure);

                Add(uapJob);
                Add(new TagColumn("Tool", name => name.Replace(GetMetric(name), "")));
                Add(new TagColumn("Metric", GetMetric));
            }

            private static string GetMetric(string name)
            {
                return name.Contains("Latency") ? "Latency" : "Granularity";
            }
        }

        [Benchmark]
        public long UtcNowLatency()
        {
            return DateTime.UtcNow.Ticks;
        }

        [Benchmark]
        public long NowLatency()
        {
            return DateTime.Now.Ticks;
        }

        [Benchmark]
        public long UtcNowGranularity()
        {
            long lastTicks = DateTime.UtcNow.Ticks;
            while (DateTime.UtcNow.Ticks == lastTicks)
            {
            }
            return lastTicks;
        }

        [Benchmark]
        public long NowGranularity()
        {
            long lastTicks = DateTime.Now.Ticks;
            while (DateTime.Now.Ticks == lastTicks)
            {
            }
            return lastTicks;
        }
    }
}
