using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using ICustomAttribute = ZenPlatform.Compiler.Contracts.ICustomAttribute;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibType : IType
    {
        private readonly DnlibAssembly _assembly;

        public DnlibType(TypeDef typeDefinition, DnlibAssembly assembly)
        {
            _assembly = assembly;
            TypeDefinition = typeDefinition;
        }

        public TypeDef TypeDefinition { get; }

        public bool Equals(IType other)
        {
            throw new NotImplementedException();
        }

        public object Id => TypeDefinition.FullName;
        public string Name => TypeDefinition.Name;
        public string Namespace => TypeDefinition.Namespace;
        public string FullName => TypeDefinition.FullName;
        public IAssembly Assembly => _assembly;

        private IReadOnlyList<IProperty> _properties;
        private IReadOnlyList<IField> _fields;
        private IReadOnlyList<IMethod> _methods;
        private IReadOnlyList<IConstructor> _constructors;

        public IReadOnlyList<IProperty> Properties =>
            _properties ??=TypeDefinition.Properties.Select(x => new DnlibProperty(x)).ToList();

        public IReadOnlyList<IField> Fields =>
            _fields ??= TypeDefinition.Fields.Select(x => new DnlibField(x)).ToList();

        public IReadOnlyList<IEventInfo> Events { get; }

        public IReadOnlyList<IMethod> Methods =>
            _methods ??= TypeDefinition.Methods.Select(x => new DnlibMethod(x)).ToList();

        public IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= TypeDefinition.FindConstructors().Select(x => new DnlibConstructor(x)).ToList();

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