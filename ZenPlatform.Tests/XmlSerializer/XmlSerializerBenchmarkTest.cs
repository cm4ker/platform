using BenchmarkDotNet.Running;
using Xunit;

namespace ZenPlatform.Tests.XmlSerializer
{
    public class XmlSerializerBenchmarkTest
    {
        [Fact]
        public void Benchmark1()
        {
            BenchmarkRunner.Run<XmlSerializerBenchmark>();
        }
    }
}