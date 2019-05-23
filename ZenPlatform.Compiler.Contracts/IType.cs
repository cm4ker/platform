using System;
using System.Collections.Generic;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IType : IEquatable<IType>
    {
        object Id { get; }
        string Name { get; }
        string Namespace { get; }
        string FullName { get; }
        IAssembly Assembly { get; }
        IReadOnlyList<IProperty> Properties { get; }
        IReadOnlyList<IEventInfo> Events { get; }
        IReadOnlyList<IField> Fields { get; }
        IReadOnlyList<IMethod> Methods { get; }
        IReadOnlyList<IConstructor> Constructors { get; }
        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
        IReadOnlyList<IType> GenericArguments { get; }
        bool IsAssignableFrom(IType type);
        IType MakeGenericType(IReadOnlyList<IType> typeArguments);
        IType GenericTypeDefinition { get; }
        bool IsArray { get; }
        IType ArrayElementType { get; }
        IType MakeArrayType(int dimensions);
        IType BaseType { get; }
        bool IsValueType { get; }
        bool IsEnum { get; }
        IReadOnlyList<IType> Interfaces { get; }
        bool IsInterface { get; }
        bool IsSystem { get; }

        IType GetEnumUnderlyingType();
        IReadOnlyList<IType> GenericParameters { get; }
        int GetHashCode();
    }
}