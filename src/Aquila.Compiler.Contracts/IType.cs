using System;
using System.Collections.Generic;

namespace Aquila.Compiler.Contracts
{
    public interface IType : IEquatable<IType>
    {
        ITypeSystem TypeSystem { get; }

        /// <summary>
        /// Identifier of the type 
        /// </summary>
        object Id { get; }

        /// <summary>
        /// Name of the type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Namespace of the type
        /// </summary>
        string Namespace { get; }


        /// <summary>
        /// Full name of the type 
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Parent assembly for this type 
        /// </summary>
        IAssembly Assembly { get; }

        /// <summary>
        /// Properties
        /// </summary>
        IReadOnlyList<IProperty> Properties { get; }

        /// <summary>
        /// Events 
        /// </summary>
        IReadOnlyList<IEventInfo> Events { get; }


        /// <summary>
        /// Fields
        /// </summary>
        IReadOnlyList<IField> Fields { get; }

        /// <summary>
        /// Methods
        /// </summary>
        IReadOnlyList<IMethod> Methods { get; }


        /// <summary>
        /// Constructors 
        /// </summary>
        IReadOnlyList<IConstructor> Constructors { get; }

        /// <summary>
        /// Custom  attributes
        /// </summary>
        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }

        /// <summary>
        /// Generic arguments
        /// </summary>
        IReadOnlyList<IType> GenericArguments { get; }

        IType MakeGenericType(IReadOnlyList<IType> typeArguments);

        IType GenericTypeDefinition { get; }

        bool IsArray { get; }

        IType ArrayElementType { get; }

        IType MakeArrayType();

        IType MakeArrayType(int dimensions);

        IType BaseType { get; }

        bool IsValueType { get; }

        bool IsEnum { get; }

        IReadOnlyList<IType> Interfaces { get; }

        bool IsInterface { get; }

        bool IsSystem { get; }

        bool IsPrimitive { get; }

        bool HasGenericParameters { get; }

        IType GetEnumUnderlyingType();

        int GetHashCode();
    }
}