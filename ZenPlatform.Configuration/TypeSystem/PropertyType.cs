using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class PropertyType : IPropertyType
    {
        public PropertyType(TypeManager ts)
        {
        }

        public Guid PropertyId { get; set; }
        public Guid TypeId { get; set; }
        public ITypeManager TypeManager { get; }
    }
}