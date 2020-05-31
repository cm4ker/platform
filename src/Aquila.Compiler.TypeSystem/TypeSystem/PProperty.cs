using System;
using Aquila.Core.Contracts.TypeSystem;
using IPType = Aquila.Core.Contracts.TypeSystem.IPType;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public abstract class PProperty : PMember, IPProperty
    {
        internal PProperty(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public virtual bool IsSelfLink => false;

        public virtual bool IsSystem => false;

        public virtual bool IsUnique => false;

        public virtual bool IsReadOnly => false;

        public virtual IPType Type => TypeManager.Unknown;

        public virtual IPMethod Getter => null;

        public virtual IPMethod Setter => null;
    }
}