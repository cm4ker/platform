using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Compiler.Contracts;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibTypeBuilder : DnlibType, ITypeBuilder
    {
        private readonly DnlibTypeSystem _ts;

        public DnlibTypeBuilder(DnlibTypeSystem typeSystem, TypeDef typeDef, DnlibAssembly assembly)
            : base(typeSystem, typeDef, typeDef, assembly)
        {
            _ts = typeSystem;
            Methods.Any();
        }

        public void AddInterfaceImplementation(IType type)
        {
            var dType = (DnlibType) type;
            this.TypeDef.Interfaces.Add(new InterfaceImplUser(dType.TypeRef));
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, GenericParameterConstraint>> names)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IMethodBuilder> DefinedMethods => Methods.Cast<IMethodBuilder>().ToList();

        public IField DefineField(IType type, string name, bool isPublic, bool isStatic)
        {
            var field = new FieldDefUser(name);
            field.IsStatic = isStatic;
            field.Access |= (isPublic) ? FieldAttributes.Public : FieldAttributes.Private;
            TypeDef.Fields.Add(field);

            return new DnlibField(field);
        }

        public IMethodBuilder DefineMethod(string name, bool isPublic, bool isStatic, bool isInterfaceImpl,
            IMethod overrideMethod = null)
        {
            var method = new MethodDefUser(name);
            
            var dm = new DnlibMethodBuilder(_ts, method, TypeRef);
            ((List<IMethod>) Methods).Add(dm);
            
            method.Attributes |= (isPublic) ? MethodAttributes.Public : MethodAttributes.Private;

            method.IsStatic = isStatic;
            if (isInterfaceImpl)
                method.Attributes |= MethodAttributes.NewSlot | MethodAttributes.Virtual;

            method.DeclaringType = TypeDef;
            method.Body = new CilBody();

            method.MethodSig = new MethodSig();

            

            return dm;
        }

        public IPropertyBuilder DefineProperty(IType propertyType, string name)
        {
            var prop = new PropertyDefUser(name);


            TypeDef.Properties.Add(prop);
            prop.DeclaringType = ((DnlibType) propertyType).TypeDef;


            var propertyBuilder = new DnlibPropertyBuilder(_ts, prop);
            ((List<DnlibProperty>) Properties).Add(propertyBuilder);

            return propertyBuilder;
        }

        public IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(new ClassSig(_ts.GetSystemBindings().Void.GetRef()));
            else
                sig = MethodSig.CreateInstance(new ClassSig(_ts.GetSystemBindings().Void.GetRef()));

            foreach (var arg in args)
            {
                sig.Params.Add(new ClassSig(((DnlibType) arg).TypeRef));
            }

            var name = (isStatic) ? ".cctor" : ".ctor";
            var c = new MethodDefUser(name, sig);

            c.DeclaringType = TypeDef;

            return new DnlibConstructorBuilder(_ts, c, TypeRef);
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