using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.Uap;
using System.IO;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkDotNet.Samples.Framework
{
    [UapJob("https://192.168.1.51/", "2LYMHOZH9C4e1PUcBQVkkNDFikCXpY6v", "3914980555611581204597258868819265487056560409287773938948508430", @"D:\Workshop\BenchmarkDotNet\src\BenchmarkDotNet\bin\Debug\uap10.0")]
    public class Framework_DateTime
    {
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
