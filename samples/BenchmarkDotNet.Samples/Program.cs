using System.Reflection;
using BenchmarkDotNet.Running;

#if UAP
namespace System.Threading
{
    internal static class Thread
    {
        public static void Sleep(int milliseconds)
        {
            Tasks.Task.Delay(milliseconds).Wait();
        }
    }
}
#endif