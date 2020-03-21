using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Common.TypeSystem
{
    public sealed class PTypeSpec : PType, IPTypeSpec
    {
        private readonly IPType _ipType;
        private string _name;


        internal PTypeSpec(IPType ipType, TypeManager ts) : base(ts)
        {
            _ipType = ipType;
            Id = Guid.NewGuid();
        }

        public IPType BaseType => _ipType;
        public override string Name => _name ??= CalcName();
        
        public override Guid? BaseId => _ipType.Id;

        public int Scale { get; set; }

        public int Precision { get; set; }

        public int Size { get; set; }

        public override bool IsTypeSpec => true;

        public override bool IsPrimitive => _ipType.IsPrimitive;

        private string CalcName()
        {
            var result = _ipType.Name;

            if (_ipType.IsSizable)
                result += $"({Size})";
            if (_ipType.IsScalePrecision)
                result += $"({Scale},{Precision})";

            return result;
        }

        private bool Equals(PTypeSpec other)
        {
            return Equals(_ipType, other._ipType) && _name == other._name && Scale == other.Scale &&
                   Precision == other.Precision && Size == other.Size;
        }

        public bool Equals(IPType other)
        {
            if (other is PTypeSpec ts) return Equals(ts);
            return false;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is PTypeSpec other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_ipType, _name, Scale, Precision, Size);
        }
    }
}