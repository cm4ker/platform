using System;
using System.Collections.Generic;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibTypeBuilder : DnlibType, ITypeBuilder
    {
        public DnlibTypeBuilder(TypeDef typeDef, DnlibAssembly assembly) : base(typeDef, assembly)
        {
        }

        public void AddInterfaceImplementation(IType type)
        {
            throw new NotImplementedException();
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> names)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IMethodBuilder> DefinedMethods { get; }

        public IField DefineField(IType type, string name, bool isPublic, bool isStatic)
        {
            throw new NotImplementedException();
        }

        public IMethodBuilder DefineMethod(string name, bool isPublic, bool isStatic, bool isInterfaceImpl,
            IMethod overrideMethod = null)
        {
            throw new NotImplementedException();
        }

        public IPropertyBuilder DefineProperty(IType propertyType, string name)
        {
            throw new NotImplementedException();
        }

        public IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args)
        {
            throw new NotImplementedException();
        }

        public ITypeBuilder DefineNastedType(IType baseType, string name, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public IType EndBuild()
        {
            throw new NotImplementedException();
        }
    }

    public class DnlibField : IField
    {
        public FieldDef FieldDef { get; }

        public DnlibField(FieldDef fieldDef)
        {
            FieldDef = fieldDef;
        }

        public bool Equals(IField other)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public IType FieldType { get; }
        public bool IsPublic { get; }
        public bool IsStatic { get; }
        public bool IsLiteral { get; }

        public object GetLiteralValue()
        {
            throw new NotImplementedException();
        }
    }
}