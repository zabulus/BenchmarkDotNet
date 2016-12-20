using BenchmarkDotNet.Running;
using BenchmarkDotNet.Samples.Framework;
using System.Reflection;

namespace BenchmarkDotNet.Samples.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Framework_DateTime).GetTypeInfo().Assembly).Run(args);
        }
    }
}
