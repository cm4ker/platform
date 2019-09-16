using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class UnresolvedDnlibType : IType
    {
        public UnresolvedDnlibType(TypeRef reference)
        {
        }

        public bool Equals(IType other)
        {
            throw new NotImplementedException();
        }

        public object Id { get; }
        public string Name { get; }
        public string Namespace { get; }
        public string FullName { get; }
        public IAssembly Assembly { get; }
        public IReadOnlyList<IProperty> Properties { get; }
        public IReadOnlyList<IEventInfo> Events { get; }
        public IReadOnlyList<IField> Fields { get; }
        public IReadOnlyList<IMethod> Methods { get; }
        public IReadOnlyList<IConstructor> Constructors { get; }
        public IReadOnlyList<ICustomAttribute> CustomAttributes { get; }
        public IReadOnlyList<IType> GenericArguments { get; }
        public bool IsAssignableFrom(IType type)
        {
            throw new NotImplementedException();
        }

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            throw new NotImplementedException();
        }

        public IType GenericTypeDefinition { get; }
        public bool IsArray { get; }
        public IType ArrayElementType { get; }
        public IType MakeArrayType()
        {
            throw new NotImplementedException();
        }

        public IType MakeArrayType(int dimensions)
        {
            throw new NotImplementedException();
        }

        public IType BaseType { get; }
        public bool IsValueType { get; }
        public bool IsEnum { get; }
        public IReadOnlyList<IType> Interfaces { get; }
        public bool IsInterface { get; }
        public bool IsSystem { get; }
        public IType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IType> GenericParameters { get; }
    }
}