using Aquila.Metadata;

namespace Aquila.Syntax.Metadata
{
    /// <summary>
    /// Response for reading metadata and decide what i can do with it
    /// </summary>
    public class MetadataReader
    {
        public void Parse(string yaml)
        {
            var d = new YamlDotNet.Serialization.Deserializer();
            var instance = d.Deserialize<MetadataBase>(yaml);

            //TODO: create
        }
    }
}