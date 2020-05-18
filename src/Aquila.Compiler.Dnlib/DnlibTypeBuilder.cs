using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Aquila.Compiler.Contracts;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IField = Aquila.Compiler.Contracts.IField;
using IMethod = Aquila.Compiler.Contracts.IMethod;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibTypeBuilder : DnlibType, ITypeBuilder
    {
        private readonly DnlibTypeSystem _ts;
        private DnlibContextResolver _r;

        public DnlibTypeBuilder(DnlibTypeSystem typeSystem, TypeDef typeDef, DnlibAssembly assembly)
            : base(typeSystem, typeDef, typeDef, assembly)
        {
            _ts = typeSystem;

            Methods.Any();
            Properties.Any();
            Constructors.Any();
            CustomAttributes.Any();
            //GenericParameters.Any();

            _r = new DnlibContextResolver(_ts, typeDef.Module);
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
            var tref = _r.GetReference(((DnlibType) type).TypeRef);
            var field = new FieldDefUser(name, new FieldSig(tref.ToTypeSig()));
            field.IsStatic = isStatic;
            field.Access |= (isPublic) ? FieldAttributes.Public : FieldAttributes.Private;
            field.DeclaringType = TypeDef;

            var dfield = new DnlibField(_ts, field);

            ((List<DnlibField>) Fields).Add(dfield);

            return dfield;
        }

        public IMethodBuilder DefineMethod(string name, bool isPublic, bool isStatic, bool isInterfaceImpl,
            IMethod overrideMethod = null, bool isVirtual = false)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());
            else
                sig = MethodSig.CreateInstance(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());

            var method = new MethodDefUser(name, sig);

            var dm = new DnlibMethodBuilder(_ts, method, TypeRef);
            ((List<IMethod>) Methods).Add(dm);


            method.Attributes |= (isPublic) ? MethodAttributes.Public : MethodAttributes.Private;

            method.IsStatic = isStatic;

            if (overrideMethod != null)
            {
                var mo = new MethodOverride(method, (IMethodDefOrRef) ((DnlibMethodBase) overrideMethod).MethodRef);
                method.Overrides.Add(mo);
                method.Attributes |= MethodAttributes.Virtual;
            }

            if (isInterfaceImpl)
            {
                method.Attributes |= MethodAttributes.NewSlot | MethodAttributes.Virtual;
            }

            if (isVirtual)
                method.Attributes |= MethodAttributes.Virtual;

            method.Attributes |= MethodAttributes.HideBySig;

            method.DeclaringType = TypeDef;
            method.Body = new CilBody();

            method.Body.InitLocals = true;
            //method.MethodSig = new MethodSig(c);

            method.ReturnType = _r.GetReference(_ts.GetSystemBindings().Void.ToTypeRef()).ToTypeSig();

            return dm;
        }

        public IPropertyBuilder DefineProperty(IType propertyType, string name, bool isStatic = false)
        {
            var prop = new PropertyDefUser(name);

            TypeDef.Properties.Add(prop);
            prop.DeclaringType = TypeDef;

            if (isStatic)
                prop.PropertySig = PropertySig.CreateStatic(_r.GetReference(propertyType.GetRef()).ToTypeSig(),
                    _r.GetReference(propertyType.GetRef()).ToTypeSig());
            else
                prop.PropertySig = PropertySig.CreateInstance(_r.GetReference(propertyType.GetRef()).ToTypeSig(),
                    _r.GetReference(propertyType.GetRef()).ToTypeSig());

            var propertyBuilder = new DnlibPropertyBuilder(_ts, prop, TypeRef);
            ((List<DnlibProperty>) Properties).Add(propertyBuilder);

            return propertyBuilder;
        }

        public IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args)
        {
            MethodSig sig;

            if (isStatic)
                sig = MethodSig.CreateStatic(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());
            else
                sig = MethodSig.CreateInstance(_ts.GetSystemBindings().Void.GetRef().ToTypeSig());

            foreach (var arg in args)
            {
                sig.Params.Add(arg.GetRef().ToTypeSig());
            }

            var name = (isStatic) ? ".cctor" : ".ctor";
            var c = new MethodDefUser(name, sig);

            c.IsStatic = isStatic;
            
            c.Attributes |= MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public |
                            MethodAttributes.HideBySig;

            c.DeclaringType = TypeDef;

            var dc = new DnlibConstructorBuilder(_ts, c, TypeRef);
            ((List<IConstructor>) Constructors).Add(dc);

            return dc;
        }

        public ITypeBuilder DefineNastedType(IType baseType, string name, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public void SetCustomAttribute(ICustomAttribute attr)
        {
            var dnlibAttr = (DnlibCustomAttribute) attr;
            dnlibAttr.ImportAttribute(TypeDef.Module);
            ((List<ICustomAttribute>) CustomAttributes).Add(dnlibAttr);
            TypeDef.CustomAttributes.Add(dnlibAttr.GetCA());
        }

        public IType EndBuild()
        {
            throw new NotImplementedException();
        }
    }
}