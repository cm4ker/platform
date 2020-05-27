using System;
using Aquila.Core.Contracts.TypeSystem;
using IPType = Aquila.Core.Contracts.TypeSystem.IPType;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PProperty : PMember, IPProperty
    {
        private Guid _typeId;

        internal PProperty(Guid parentId, TypeManager ts) : this(Guid.NewGuid(), parentId, ts)
        {
        }

        internal PProperty(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsUnique { get; set; }

        public bool IsReadOnly { get; set; }

        public IPType Type => TypeManager.FindType(_typeId);

        public void SetType(Guid guid)
        {
            _typeId = guid;
        }
    }
}