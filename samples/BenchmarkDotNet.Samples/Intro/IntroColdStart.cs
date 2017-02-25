﻿using System;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;

namespace BenchmarkDotNet.Samples.Intro
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 5)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class IntroColdStart
    {
        private bool firstCall;

        [Benchmark]
        public void Foo()
        {
            if (firstCall == false)
            {
                firstCall = true;
                Console.WriteLine("// First call");
                Thread.Sleep(1000);
            }
            else
                Thread.Sleep(10);
        }
    }
}