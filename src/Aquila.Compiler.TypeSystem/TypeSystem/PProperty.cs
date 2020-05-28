using System;
using Aquila.Core.Contracts.TypeSystem;
using IPType = Aquila.Core.Contracts.TypeSystem.IPType;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PProperty : PMember, IPProperty
    {
        private Guid _typeId;
        private Guid _getterId;
        private Guid _setterId;

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

        public IPMethod Getter => TypeManager.FindMethod(_getterId);

        public IPMethod Setter => TypeManager.FindMethod(_setterId);

        public void SetGetter(Guid id)
        {
            _getterId = id;
        }

        public void SetSetter(Guid id)
        {
            _setterId = id;
        }

        public void SetType(Guid guid)
        {
            _typeId = guid;
        }
    }
}