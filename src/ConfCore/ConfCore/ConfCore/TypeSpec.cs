using System;

namespace ConfCore
{
    public class TypeSpec : Type
    {
        private readonly Type _type;
        private string _name;


        internal TypeSpec(Type type, TypeSystem ts) : base(ts)
        {
            _type = type;
        }

        public override string Name => _name ??= CalcName();

        public override Guid? ParentId => _type.Id;

        public int Scale { get; set; }

        public int Precision { get; set; }

        public int Size { get; set; }

        public bool IsNullable { get; set; }

        public override bool IsTypeSpec => true;

        private string CalcName()
        {
            var result = _type.Name;

            if (IsNullable)
                result += "?";
            if (_type.IsSizable)
                result += $"({Size})";
            if (_type.IsScalePrecision)
                result += $"({Scale},{Precision})";

            return result;
        }
    }
}