using System;
using System.Collections.Generic;

//using Aquila.AsmClientInfrastructure;

namespace Aquila.Compiler.Contracts
{
    public class UnknownType : IType
    {
        private readonly ITypeSystem _ts;

        public UnknownType(string name)
        {
            Name = name;
        }

        public bool Equals(IType other) => other == this;

        public ITypeSystem TypeSystem => _ts;

        public object Id { get; } = Guid.NewGuid();

        public string Name { get; protected set; }
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

        public virtual IType MakeArrayType() => throw new NotImplementedException();

        public virtual IType MakeArrayType(int dimensions) => throw new NullReferenceException();

        public IType BaseType { get; }

        public bool IsValueType { get; } = false;

        public bool IsEnum { get; } = false;

        public IReadOnlyList<IType> Interfaces { get; } = new IType[0];

        public bool IsInterface => false;

        public bool IsSystem { get; }

        public bool IsPrimitive { get; }

        public IType GetEnumUnderlyingType() => throw new InvalidOperationException();

        public IReadOnlyList<IType> GenericParameters { get; } = null;

        public bool IsAssignableFrom(IType type) => type == this;

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            throw new NotSupportedException();
        }

        public IType GenericTypeDefinition => null;
        public bool IsArray { get; protected set; }
        public IType ArrayElementType { get; protected set; }

        public static readonly IType Unknown = new UnknownType("{Unknown type}");
    }

    public class UnknownArrayType : UnknownType
    {
        public UnknownArrayType(string name, IType arrayElemType) : base(name)
        {
            ArrayElementType = arrayElemType;
            IsArray = true;

            if (!(arrayElemType is UnknownType))
            {
                Name = $"{arrayElemType.Name}[]";
            }
        }

        public override IType MakeArrayType()
        {
            return ArrayElementType.MakeArrayType();
        }

        public override IType MakeArrayType(int dimensions)
        {
            return ArrayElementType.MakeArrayType(dimensions);
        }
    }
}