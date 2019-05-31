using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    [DebuggerDisplay("{" + nameof(Type) + "}")]
    class SreType : SreMemberInfo, IType
    {
        private IReadOnlyList<IProperty> _properties;
        private IReadOnlyList<IField> _fields;
        private IReadOnlyList<IMethod> _methods;
        private IReadOnlyList<IConstructor> _constructors;
        private IReadOnlyList<IType> _genericArguments;
        private IReadOnlyList<IType> _genericParameters;
        private IReadOnlyList<IType> _interfaces;
        private IReadOnlyList<IEventInfo> _events;
        public Type Type { get; }

        public SreType(SreTypeSystem system, SreAssembly asm, Type type) : base(system, type)
        {
            Assembly = asm;
            Type = type;
        }

        public bool Equals(IType other) => Type == (other as SreType)?.Type;
        public override int GetHashCode() => Type.GetHashCode();
        public object Id => Type;

        public string FullName => Type.FullName;
        public string Namespace => Type.Namespace;
        public IAssembly Assembly { get; }

        public IReadOnlyList<IProperty> Properties =>
            _properties ??= Type
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance |
                               BindingFlags.NonPublic)
                .Select(p => new SreProperty(System, p)).ToList();

        public IReadOnlyList<IField> Fields =>
            _fields ??= Type.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
                .Select(f => new SreField(System, f)).ToList();

        public IReadOnlyList<IMethod> Methods =>
            _methods ??= Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static |
                                         BindingFlags.Instance)
                .Select(m => new SreMethod(System, m)).ToList();

        public IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= Type.GetConstructors(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Select(c => new SreConstructor(System, c)).ToList();

        public IReadOnlyList<IType> Interfaces =>
            _interfaces ?? (_interfaces = Type.GetInterfaces().Select(System.ResolveType).ToList());

        public IReadOnlyList<IEventInfo> Events =>
            _events ?? (_events = Type
                .GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Select(e => new SreEvent(System, e)).ToList());

        public bool IsInterface => Type.IsInterface;
        public bool IsSystem { get; }

        public IReadOnlyList<IType> GenericArguments
        {
            get
            {
                if (_genericArguments != null)
                    return _genericArguments;
                if (GenericTypeDefinition == null)
                    return _genericArguments = new IType[0];
                return _genericArguments = Type.GetGenericArguments().Select(System.ResolveType).ToList();
            }
        }

        public IReadOnlyList<IType> GenericParameters =>
            _genericParameters ?? (_genericParameters =
                Type.GetTypeInfo().GenericTypeParameters.Select(System.ResolveType).ToList());

        public bool IsAssignableFrom(IType type)
        {
            return Type.IsAssignableFrom(((SreType) type).Type);
        }

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            return System.ResolveType(
                Type.MakeGenericType(typeArguments.Select(t => ((SreType) t).Type).ToArray()));
        }

        public IType GenericTypeDefinition => Type.IsConstructedGenericType
            ? System.ResolveType(Type.GetGenericTypeDefinition())
            : null;

        public bool IsArray => Type.IsArray;
        public IType ArrayElementType => IsArray ? System.ResolveType(Type.GetElementType()) : null;

        public IType MakeArrayType()
        {
            return System.ResolveType(Type.MakeArrayType());
        }

        public IType MakeArrayType(int dimensions) => System.ResolveType(
            dimensions == 1 ? Type.MakeArrayType() : Type.MakeArrayType(dimensions));

        public IType BaseType => Type.BaseType == null ? null : System.ResolveType(Type.BaseType);
        public bool IsValueType => Type.IsValueType;
        public bool IsEnum => Type.IsEnum;

        public IType GetEnumUnderlyingType()
        {
            return System.ResolveType(Enum.GetUnderlyingType(Type));
        }
    }
}