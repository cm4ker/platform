using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public class PseudoType : IType
    {
        public PseudoType(string name)
        {
            Name = name;
        }

        public bool Equals(IType other) => other == this;

        public object Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public string Namespace { get; } = "";
        public string FullName => Name;
        public IAssembly Assembly { get; } = null;
        public IReadOnlyList<IProperty> Properties { get; } = new IProperty[0];
        public IReadOnlyList<IEventInfo> Events { get; } = new IEventInfo[0];
        public IReadOnlyList<IField> Fields { get; } = new List<IField>();
        public IReadOnlyList<IMethod> Methods { get; } = new IMethod[0];
        public IReadOnlyList<IConstructor> Constructors { get; } = new IConstructor[0];
        public IReadOnlyList<ICustomAttribute> CustomAttributes { get; } = new ICustomAttribute[0];
        public IReadOnlyList<IType> GenericArguments { get; } = new IType[0];
        public IType MakeArrayType(int dimensions) => throw new NullReferenceException();

        public IType BaseType { get; }
        public bool IsValueType { get; } = false;
        public bool IsEnum { get; } = false;
        public IReadOnlyList<IType> Interfaces { get; } = new IType[0];
        public bool IsInterface => false;
        public bool IsSystem { get; }
        public IType GetEnumUnderlyingType() => throw new InvalidOperationException();
        public IReadOnlyList<IType> GenericParameters { get; } = null;

        public bool IsAssignableFrom(IType type) => type == this;

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            throw new NotSupportedException();
        }

        public IType GenericTypeDefinition => null;
        public bool IsArray { get; }
        public IType ArrayElementType { get; }
        public static PseudoType Null { get; } = new PseudoType("{x:Null}");
        public static PseudoType Unknown { get; } = new PseudoType("{Unknown type}");
    }
}