using System.Collections.Generic;
using System.Data;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aquila.Metadata
{
    public sealed class EntityMetadata : MetadataBase
    {
        public EntityMetadata()
        {
            Properties = new List<EntityProperty>();
        }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }

        public List<EntityProperty> Properties { get; set; }


        private static readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        private static readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
            .Build();

        public static EntityMetadata FromYaml(string yaml)
        {
            return Deserializer.Deserialize<EntityMetadata>(yaml);
        }

        public static string ToYaml(EntityMetadata md)
        {
            return Serializer.Serialize(md);
        }
    }
}