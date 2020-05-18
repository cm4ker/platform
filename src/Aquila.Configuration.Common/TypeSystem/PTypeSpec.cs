using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    public sealed class PTypeSpec : PType, IPTypeSpec
    {
        private readonly Guid _baseId;

        private bool _isArray;
        private string _name;


        internal PTypeSpec(Guid baseId, TypeManager ts) : base(ts)
        {
            _baseId = baseId;
            Id = Guid.NewGuid();
        }

        private IPType GetBase()
        {
            return TypeManager.FindType(_baseId) ?? throw new Exception("Type not found");
        }

        public IPType BaseType => GetBase();
        public override string Name => _name ??= CalcName();

        public override Guid? BaseId => _baseId;

        public int Scale { get; set; }

        public int Precision { get; set; }

        public int Size { get; set; }

        public void SetScale(int value)
        {
            Scale = value;
        }

        public void SetPrecision(int value)
        {
            Precision = value;
        }

        public void SetSize(int value)
        {
            Size = value;
        }

        public void SetIsArray(bool value)
        {
            _isArray = value;
        }

        public override bool IsArray => _isArray;

        public override bool IsTypeSpec => true;

        public override bool IsPrimitive => GetBase().IsPrimitive;

        public override PrimitiveKind PrimitiveKind => GetBase().PrimitiveKind;

        private string CalcName()
        {
            var result = GetBase().Name;

            if (GetBase().IsSizable)
                result += $"({Size})";
            if (GetBase().IsScalePrecision)
                result += $"({Scale},{Precision})";

            return result;
        }

        private bool Equals(PTypeSpec other)
        {
            return Equals(_baseId, other._baseId) && _name == other._name && Scale == other.Scale &&
                   Precision == other.Precision && Size == other.Size && IsArray == other.IsArray;
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
            return HashCode.Combine(_baseId, _name, Scale, Precision, Size);
        }
    }
}