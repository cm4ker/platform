using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public class PropertyType : IPropertyType
    {
        public PropertyType(TypeManager ts)
        {
        }

        public Guid PropertyParentId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TypeId { get; set; }
        public ITypeManager TypeManager { get; }
    }
}