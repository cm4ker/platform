using System;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

namespace ZenPlatform.Core
{
    public class EntityMetadata
    {
        public EntityMetadata(XCObjectTypeBase entityConfig, Type entityType, Type dtoType)
        {
            EntityConfig = entityConfig;
            EntityType = entityType;
            DtoType = dtoType;
        }

        public Guid Key => EntityConfig.Guid;
        public XCObjectTypeBase EntityConfig { get; }
        public Type EntityType { get; }
        public Type DtoType { get; }
    }
}