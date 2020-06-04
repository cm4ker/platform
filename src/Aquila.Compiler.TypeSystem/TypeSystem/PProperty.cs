using System;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public abstract class PProperty : PMember
    {
        internal PProperty(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public virtual bool IsSelfLink => false;

        public virtual bool IsSystem => false;

        public virtual bool IsUnique => false;

        public virtual bool IsReadOnly => false;

        public virtual PType Type => TypeManager.Unknown;

        public virtual PMethod Getter => null;

        public virtual PMethod Setter => null;

        public IProperty BackendProperty { get; set; }
    }

    public abstract class PField : PMember
    {
        internal PField(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public virtual PType Type => TypeManager.Unknown;

        public IField BackendField { get; set; }
    }
}