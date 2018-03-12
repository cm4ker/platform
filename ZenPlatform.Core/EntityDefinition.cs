using System;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.Core
{
    public class EntityDefinition
    {
        public EntityDefinition(PObjectType entityConfig, Type entityType, Type dtoType)
        {
            EntityConfig = entityConfig;
            EntityType = entityType;
            DtoType = dtoType;

        }

        public Guid Key => EntityConfig.Id;
        public PObjectType EntityConfig { get; }
        public Type EntityType { get; }
        public Type DtoType { get; }
    }
}