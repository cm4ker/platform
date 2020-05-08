using System;
using System.Reflection;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Sre
{
    class SreField : SreMemberInfo, IField
    {
        public FieldInfo Field { get; }

        public SreField(SreTypeSystem system, FieldInfo field) : base(system, field)
        {
            Field = field;
            FieldType = system.ResolveType(field.FieldType);
        }

        public IType FieldType { get; }
        public bool IsPublic => Field.IsPublic;
        public bool IsStatic => Field.IsStatic;
        public bool IsLiteral => Field.IsLiteral;

        public object GetLiteralValue()
        {
            if (!IsLiteral)
                throw new InvalidOperationException();
            return Field.GetValue(null);
        }

        public bool Equals(IField other) => ((SreField) other)?.Field.Equals(Field) == true;
    }
}