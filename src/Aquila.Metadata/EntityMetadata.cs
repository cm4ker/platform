using System.Collections.Generic;

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

        public List<EntityProperty> Properties { get; }
    }
}