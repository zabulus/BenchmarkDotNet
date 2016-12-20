using BenchmarkDotNet.Jobs;

namespace BenchmarkDotNet.Attributes.Jobs
{
    public class AupJobAttribute : JobConfigBaseAttribute
    {
        public AupJobAttribute() : base(Job.Uap)
        {
        }
    }
}
