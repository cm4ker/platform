using System.Linq;
using BenchmarkDotNet.Attributes;

namespace ZenPlatform.Tests.XmlSerializer
{
    public class XmlSerializerBenchmark
    {
        [Benchmark]
        public int Test100()
        {
            return Enumerable.Range(1, 100).Sum();
        }
    }
}