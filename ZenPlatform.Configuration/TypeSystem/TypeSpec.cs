using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.TypeSystem
{
    public class TypeSpec : Type, ITypeSpec
    {
        private readonly IType _type;
        private string _name;


        internal TypeSpec(IType type, TypeManager ts) : base(ts)
        {
            _type = type;
        }

        public override string Name => _name ??= CalcName();

        public override Guid? ParentId => _type.Id;

        public int Scale { get; set; }

        public int Precision { get; set; }

        public int Size { get; set; }

        public override bool IsTypeSpec => true;

        private string CalcName()
        {
            var result = _type.Name;

            if (_type.IsSizable)
                result += $"({Size})";
            if (_type.IsScalePrecision)
                result += $"({Scale},{Precision})";

            return result;
        }
    }
}