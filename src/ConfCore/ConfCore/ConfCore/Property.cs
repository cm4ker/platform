using System;

namespace ConfCore
{
    public class Property
    {
        internal Property(TypeSystem ts)
        {
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsReadOnly { get; set; }
    }
}