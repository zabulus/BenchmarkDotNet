using BenchmarkDotNet.Jobs;

namespace BenchmarkDotNet.Attributes.Jobs
{
    public class UapJobAttribute : JobConfigBaseAttribute
    {
        public UapJobAttribute() : base(Job.Uap)
        {
        }
    }
}
