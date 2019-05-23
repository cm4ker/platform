using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;


namespace ZenPlatform.Compiler.Cecil
{
    class CecilProperty : IProperty
    {
        private readonly TypeReference _declaringType;
        public CecilTypeSystem TypeSystem { get; }
        public PropertyDefinition Property { get; }

        public CecilProperty(CecilTypeSystem typeSystem, PropertyDefinition property, TypeReference declaringType)
        {
            _declaringType = declaringType;
            TypeSystem = typeSystem;
            Property = property;
        }

        public bool Equals(IProperty other) => other is CecilProperty cp && cp.Property == Property;

        public string Name => Property.Name;
        private IType _type;

        public IType PropertyType =>
            _type ?? (_type = TypeSystem.Resolve(Property.PropertyType.TransformGeneric(_declaringType)));

        private IMethod _setter;

        public IMethod Setter => Property.SetMethod == null
            ? null
            : _setter ?? (_setter = TypeSystem.Resolve(Property.SetMethod, _declaringType));

        private IMethod _getter;

        public IMethod Getter => Property.GetMethod == null
            ? null
            : _getter ?? (_getter = TypeSystem.Resolve(Property.GetMethod, _declaringType));

        private IReadOnlyList<ICustomAttribute> _attributes;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ?? (_attributes =
                Property.CustomAttributes.Select(ca => new CecilCustomAttribute(TypeSystem, ca)).ToList());
    }
}