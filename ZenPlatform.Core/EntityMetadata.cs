using System;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core
{
    public class EntityMetadata
    {
        public EntityMetadata(IXCObjectType entityConfig, Type entityType, Type dtoType)
        {
            EntityConfig = entityConfig;
            EntityType = entityType;
            DtoType = dtoType;
        }

        public Guid Key => EntityConfig.Guid;
        public IXCObjectType EntityConfig { get; }
        public Type EntityType { get; }
        public Type DtoType { get; }
    }
}