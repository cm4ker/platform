using System;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class PropertyType
    {
        public PropertyType(TypeSystem ts)
        {
        }

        public Guid PropertyId { get; set; }
        public Guid TypeId { get; set; }
    }
}