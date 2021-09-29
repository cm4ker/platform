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
Name: Author
Properties:

#Name field
- Name: Name
  Types:
  - Name : string
    Size : 100

#SecondName field
- Name: SecondName
  Types:
  - Name : string
    Size : 100

#BirthDay field
- Name: Birthday
  Types: 
    - Name: datetime

- Name: Country
  Types:
  - Name: Entity.CountryLink

";

            var instance = EntityMetadata.FromYaml(yaml);
            var text = EntityMetadata.ToYaml(instance);

            Assert.Equal("Author", instance.Name);
        }

        [Fact]
        public void MdLoadTest()
        {
            
        }
    }
}