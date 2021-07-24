using Aquila.Metadata;
using Aquila.Syntax.Metadata;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Aquila.Compiler.Test2
{
    public class ComponentMetadata : MetadataBase
    {
        public string SomeProp { get; set; }
    }

    public class MetadataTest
    {
        [Fact]
        public void MdParseTest()
        {
            var yaml =
                @"
ComponentName: Custom component
SomeProp: Add value
";
            var d = new YamlDotNet.Serialization.DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            var instance = d.Deserialize<MetadataBase>(yaml);


            Assert.Equal("Custom component", instance.ComponentName);
        }
    }
}