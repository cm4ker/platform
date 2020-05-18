using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public sealed class PTypeSet : PType, IPTypeSet
    {
        private readonly List<Guid> _types;

        internal PTypeSet(IEnumerable<IPType> types, TypeManager ts) : this(types.Select(x => x.Id), ts)
        {
        }

        internal PTypeSet(IEnumerable<Guid> types, TypeManager ts) : this(ts)
        {
            _types.AddRange(types);
        }

        internal PTypeSet(TypeManager ts) : base(ts)
        {
            Id = Guid.NewGuid();
            _types = new List<Guid>();
        }

        public override bool IsTypeSet =>
            (_types.Count > 1) ? true : throw new Exception("TypeSet must contains two and more types");

        public override bool IsTypeSpec => true;

        public IEnumerable<IPType> Types => _types.Select(x => TypeManager.FindType(x));

        public void AddType(Guid typeId)
        {
            _types.Add(typeId);
        }

        public bool Equals(IPType other)
        {
            if (other is PTypeSet ts)
                return Equals(_types, ts._types);

            return false;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is PTypeSet other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_types != null ? _types.GetHashCode() : 0);
        }
    }
}