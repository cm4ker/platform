using Mono.Cecil;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Cecil
{
    public class CecilField : IField
    {
        private readonly FieldDefinition _def;
        public CecilTypeSystem TypeSystem { get; }
        public FieldReference Field { get; }

        public CecilField(CecilTypeSystem typeSystem, FieldDefinition def, TypeReference declaringType)
        {
            TypeSystem = typeSystem;
            _def = def;
            Field = new FieldReference(def.Name, def.FieldType, declaringType);
        }

        public bool Equals(IField other) => other is CecilField cf && cf.Field == Field;

        public string Name => Field.Name;
        private IType _type;

        public IType FieldType =>
            _type ?? (_type = TypeSystem.Resolve(Field.FieldType.TransformGeneric(Field.DeclaringType)));

        public bool IsPublic => _def.IsPublic;
        public bool IsStatic => _def.IsStatic;
        public bool IsLiteral => _def.IsLiteral;

        public object GetLiteralValue()
        {
            if (IsLiteral && _def.HasConstant)
                return _def.Constant;
            return null;
        }
    }
}