using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class Property : IProperty
    {
        internal Property(TypeManager ts)
        {
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsReadOnly { get; set; }

        public Metadata Metadata { get; set; }
    }
}