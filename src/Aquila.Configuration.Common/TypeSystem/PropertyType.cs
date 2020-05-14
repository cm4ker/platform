using System;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
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