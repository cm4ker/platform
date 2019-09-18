using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using ZenPlatform.Compiler.Contracts;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;

namespace ZenPlatform.Compiler.Cecil
{
    // TODO: Make generic type definitions have Reference set to GenericTypeInstance with parameters for
    // consistency with CecilTypeBuilder

    [DebuggerDisplay("{" + nameof(Reference) + "}")]
    class CecilType : IType, ITypeReference
    {
        private readonly CecilAssembly _assembly;
        public CecilTypeSystem TypeSystem { get; }
        public TypeReference Reference { get; }
        public TypeDefinition Definition { get; }

        public CecilType(CecilTypeSystem typeSystem, CecilAssembly assembly, TypeDefinition definition)
            : this(typeSystem, assembly, definition, definition)
        {
        }

        public CecilType(CecilTypeSystem typeSystem, CecilAssembly assembly, TypeDefinition definition,
            TypeReference reference)
        {
            _assembly = assembly;
            TypeSystem = typeSystem;
            Reference = reference;
            Definition = definition;
            if (reference.IsArray)
                Definition = ((CecilType) typeSystem.GetType("System.Array")).Definition;
        }

        public bool Equals(IType other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (!(other is CecilType o))
                return false;
            return CecilHelpers.Equals(Reference, o.Reference);
        }

        public object Id => Reference.FullName;
        public string Name => Reference.Name;
        public string FullName => Reference.FullName;
        public string Namespace => Reference.Namespace;
        public IAssembly Assembly => _assembly;
        protected IReadOnlyList<IMethod> _methods;

        public IReadOnlyList<IMethod> Methods =>
            _methods ??= Definition.GetMethods().Select(m => (IMethod) new CecilMethod(TypeSystem,
                m, Reference, _assembly.Assembly.MainModule)).ToList();

        protected IReadOnlyList<IConstructor> _constructors;

        public IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= Definition.GetConstructors().Select(c => 
                    new CecilConstructor(TypeSystem, c,
                    new MethodReference(c.Name, TypeSystem.GetTypeReference(c.ReturnType.FullName), Reference),
                    Reference))
                .ToList();

        protected IReadOnlyList<IField> _fields;

        public IReadOnlyList<IField> Fields =>
            _fields ??= Definition.Fields.Select(f => new CecilField(TypeSystem, f, Reference)).ToList();

        protected IReadOnlyList<IProperty> _properties;

        public IReadOnlyList<IProperty> Properties =>
            _properties ??= Definition.Properties.Select(p => new CecilProperty(TypeSystem, p, Reference)).ToList();

        protected IReadOnlyList<IEventInfo> _events;

        public IReadOnlyList<IEventInfo> Events =>
            _events ??= Definition.Events.Select(p => new CecilEvent(TypeSystem, p, Reference)).ToList();

        private IReadOnlyList<IType> _genericArguments;

        public IReadOnlyList<IType> GenericArguments =>
            _genericArguments ??= Reference is GenericInstanceType gi
                ? gi.GenericArguments.Select(ga => TypeSystem.Resolve(ga)).ToList()
                : null;

        private IReadOnlyList<IType> _genericParameters;

        public IReadOnlyList<IType> GenericParameters =>
            _genericParameters ?? (_genericParameters = Reference is TypeDefinition td && td.HasGenericParameters
                ? td.GenericParameters.Select(gp => TypeSystem.Resolve(gp)).ToList()
                : null);


        protected IReadOnlyList<ICustomAttribute> _attributes;

        public IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _attributes ?? (_attributes =
                Definition.CustomAttributes.Select(ca => new CecilCustomAttribute(TypeSystem, ca)).ToList());

        public bool IsAssignableFrom(IType type)
        {
            if (type.IsValueType
                && GenericTypeDefinition?.FullName == "System.Nullable`1"
                && GenericArguments[0].Equals(type))
                return true;
            if (FullName == "System.Object" && type.IsInterface)
                return true;
            var baseType = type;
            while (baseType != null)
            {
                if (baseType.Equals(this))
                    return true;
                baseType = baseType.BaseType;
            }

            if (IsInterface && type.GetAllInterfaces().Any(IsAssignableFrom))
                return true;
            return false;
        }

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            if (Reference == Definition)
            {
                var i = Definition.MakeGenericInstanceType(typeArguments.Cast<ITypeReference>()
                    .Select(r => r.Reference)
                    .ToArray());
                return TypeSystem.GetTypeFor(i);
            }

            throw new InvalidOperationException();
        }

        private IType _genericTypeDefinition;

        public IType GenericTypeDefinition =>
            _genericTypeDefinition ?? (_genericTypeDefinition =
                (Reference is GenericInstanceType) ? TypeSystem.Resolve(Definition) : null);

        public bool IsArray => Reference.IsArray;

        private IType _arrayType;

        public IType ArrayElementType =>
            _arrayType ??= IsArray ? TypeSystem.Resolve(Reference.GetElementType()) : null;

        public IType MakeArrayType() => TypeSystem.Resolve(Reference.MakeArrayType());

        public IType MakeArrayType(int dimensions) => TypeSystem.Resolve(Reference.MakeArrayType(dimensions));

        private IType _baseType;

        public IType BaseType => Definition.BaseType == null
            ? null
            : _baseType ?? (_baseType = TypeSystem.Resolve(
                  Definition.BaseType.TransformGeneric(Reference)));

        public bool IsValueType => Definition.IsValueType;
        public bool IsEnum => Definition.IsEnum;
        protected IReadOnlyList<IType> _interfaces;

        public IReadOnlyList<IType> Interfaces =>
            _interfaces ?? (_interfaces =
                Definition.Interfaces.Select(i => TypeSystem.Resolve(i.InterfaceType
                    .TransformGeneric(Reference))).ToList());

        public bool IsInterface => Definition.IsInterface;
        public bool IsSystem { get; }

        public IType GetEnumUnderlyingType()
        {
            if (!IsEnum)
                return null;
            return TypeSystem.Resolve(Definition.GetEnumUnderlyingType());
        }
    }
}