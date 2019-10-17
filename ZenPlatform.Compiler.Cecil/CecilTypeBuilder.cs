using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using ZenPlatform.Compiler.Contracts;
using GenericParameterConstraint = ZenPlatform.Compiler.Contracts.GenericParameterConstraint;

namespace ZenPlatform.Compiler.Cecil
{
    class CecilTypeBuilder : CecilType, ITypeBuilder
    {
        protected TypeReference SelfReference;
        private CecilContextResolver _cr;

        public CecilTypeBuilder(CecilTypeSystem typeSystem, CecilAssembly assembly, TypeDefinition definition)
            : base(typeSystem, assembly, definition)
        {
            SelfReference = definition;
            _methods = new List<IMethod>();
            _cr = new CecilContextResolver(typeSystem, Definition.Module);
        }

        TypeReference GetReference(IType type) => _cr.GetReference((ITypeReference) type);

        public IField DefineField(IType type, string name, bool isPublic, bool isStatic)
        {
            var r = GetReference(type);
            var attrs = default(FieldAttributes);
            if (isPublic)
                attrs |= FieldAttributes.Public;
            if (isStatic)
                attrs |= FieldAttributes.Static;

            var def = new FieldDefinition(name, attrs, r);
            Definition.Fields.Add(def);
            var rv = new CecilField(TypeSystem, def, SelfReference);
            ((List<CecilField>) Fields).Add(rv);
            return rv;
        }


        public void AddInterfaceImplementation(IType type)
        {
            Definition.Interfaces.Add(new InterfaceImplementation(GetReference(type)));
            _interfaces = null;
        }


        public IMethodBuilder DefineMethod(string name, bool isPublic,
            bool isStatic,
            bool isInterfaceImpl, IMethod overrideMethod = null)
        {
            var attrs = default(MethodAttributes);
            if (isPublic)
                attrs |= MethodAttributes.Public;
            if (isStatic)
                attrs |= MethodAttributes.Static;

            if (isInterfaceImpl)
                attrs |= MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var vType = TypeSystem.GetTypeReference("System.Void");
            var def = new MethodDefinition(name, attrs, Definition.Module.ImportReference(vType));

            if (overrideMethod != null)
                def.Overrides.Add(Definition.Module.ImportReference(((CecilMethod) overrideMethod).Definition));

            Definition.Methods.Add(def);
            var rv = new CecilMethodBuilder(TypeSystem, def, SelfReference, Definition.Module);
            ((List<IMethod>) Methods).Add(rv);
            return rv;
        }

        public IPropertyBuilder DefineProperty(IType propertyType, string name)
        {
            var def = new PropertyDefinition(name, PropertyAttributes.None,
                GetReference(propertyType));
            Definition.Properties.Add(def);
            var rv = new CecilProperty(TypeSystem, def, SelfReference);
            ((List<CecilProperty>) Properties).Add(rv);
            return rv;
        }

        public IConstructorBuilder DefineConstructor(bool isStatic, params IType[] args)
        {
            var attrs = MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

            if (isStatic)
                attrs |= MethodAttributes.Static;
            else
                attrs |= MethodAttributes.Public;

            var vType = GetReference(TypeSystem.GetSystemBindings().Void);
            var def = new MethodDefinition(isStatic ? ".cctor" : ".ctor", attrs, vType);
            if (args != null)
                foreach (var a in args)
                    def.Parameters.Add(new ParameterDefinition(GetReference(a)));
            def.Body.InitLocals = true;
            Definition.Methods.Add(def);

            var rv = new CecilConstructor(TypeSystem, def, def, SelfReference);
            ((List<CecilConstructor>) Constructors).Add(rv);
            return rv;
        }

        public ITypeBuilder DefineNastedType(IType baseType, string name, bool isPublic)
        {
            var td = new TypeDefinition("", name,
                isPublic ? TypeAttributes.NestedPublic : TypeAttributes.NestedPrivate, GetReference(baseType));

            Definition.NestedTypes.Add(td);
            return new CecilTypeBuilder(TypeSystem, (CecilAssembly) Assembly, td);
        }

        public IType EndBuild()
        {
            //Cecil not need bake the type
            return this;
        }

        public void DefineGenericParameters(IReadOnlyList<KeyValuePair<string, Mono.Cecil.GenericParameterConstraint>> args)
        {
            foreach (var arg in args)
            {
                var gp = new GenericParameter(arg.Key, Definition);
                Definition.GenericParameters.Add(gp);
            }

            Definition.Name = Name + "`" + args.Count;
            Reference.Name = Definition.Name;
            SelfReference = Definition.MakeGenericInstanceType(Definition.GenericParameters.Cast<TypeReference>()
                .ToArray());
        }

        public void DefineGenericParameters(
            IReadOnlyList<KeyValuePair<string, Contracts.GenericParameterConstraint>> names)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IMethodBuilder> DefinedMethods => Methods.Cast<IMethodBuilder>().ToList();
    }
}