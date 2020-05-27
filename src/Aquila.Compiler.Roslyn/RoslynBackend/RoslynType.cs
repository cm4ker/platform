using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using IAssembly = Aquila.Compiler.Contracts.IAssembly;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IField = Aquila.Compiler.Contracts.IField;
using IMethod = Aquila.Compiler.Contracts.IMethod;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class RoslynType : IType
    {
        private readonly RoslynTypeSystem _ts;
        private readonly RoslynAssembly _assembly;
        private readonly RoslynContextResolver _cr;

        public RoslynType(RoslynTypeSystem typeSystem, TypeDef typeDef, ITypeDefOrRef typeRef, RoslynAssembly assembly)
        {
            _ts = typeSystem;
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            TypeRef = typeRef ?? throw new ArgumentNullException(nameof(typeRef));
            TypeDef = typeDef ?? typeRef.ResolveTypeDef();

            if (typeDef is null) throw new ArgumentNullException(nameof(typeDef));
            _cr = new RoslynContextResolver(_ts, typeDef.Module);
        }

        public TypeDef TypeDef { get; }

        public ITypeDefOrRef TypeRef { get; }

        public bool Equals(RoslynType other)
        {
            return new SigComparer().Equals(TypeRef, ((RoslynType) other)?.TypeRef);
        }

        public ITypeSystem TypeSystem => _ts;

        public object Id => TypeDef.FullName;
        public string Name => TypeRef.Name;
        public string Namespace => TypeRef.Namespace;
        public string FullName => TypeRef.FullName;
        public IAssembly Assembly => _assembly;

        private IReadOnlyList<RoslynProperty> _properties;
        private IReadOnlyList<RoslynField> _fields;
        private IReadOnlyList<RoslynMethod> _methods;
        private IReadOnlyList<RoslynConstructor> _constructors;
        private IReadOnlyList<RoslynType> _interfaces;
        private IReadOnlyList<RoslynType> _genericArguments;
        private IReadOnlyList<RoslynCustomAttribute> _customAttributes;


        public virtual IReadOnlyList<IProperty> Properties =>
            _properties ??= TypeDef.Properties.Select(x => new RoslynProperty(_ts, x, TypeRef)).ToList();

        public IReadOnlyList<IEventInfo> Events { get; } = null;

        public virtual IReadOnlyList<IField> Fields =>
            _fields ??= TypeDef.Fields.Select(x => new RoslynField(_ts, x)).ToList();

        public virtual IReadOnlyList<IMethod> Methods => _methods ??= CalculateMethods();

        public virtual IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= TypeDef.FindConstructors().Select(x =>
            {
                return (RoslynConstructor) new RoslynConstructor(_ts,
                    new MemberRefUser(x.Module, x.Name, x.MethodSig, TypeRef), x,
                    TypeRef);
            }).ToList();


        public RoslynMethod CalculateMethod(MethodDef x)
        {
            return new RoslynMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, GenericArguments?.ToArray()),
                    TypeRef),
                x, TypeRef);
        }

        private List<RoslynMethod> CalculateMethods()
        {
            return TypeDef.Methods.Where(x => !x.IsConstructor)
                .Select(CalculateMethod)
                .ToList();
        }

        public virtual IReadOnlyList<ICustomAttribute> CustomAttributes =>
            _customAttributes ??= new List<RoslynCustomAttribute>();

        public virtual IReadOnlyList<IType> GenericArguments =>
            _genericArguments ??= (TypeRef as TypeSpec)?.TryGetGenericInstSig()?.GenericArguments?
                .Select(x => (RoslynType) _cr.GetType(x)).ToList();

        public virtual IReadOnlyList<IType> Interfaces =>
            _interfaces ??= TypeDef.Interfaces
                .Where(x => x.Interface.ResolveTypeDef() != null).Select(x =>
                {
                    return new RoslynType(_ts, x.Interface.ResolveTypeDef(),
                        (x.Interface.IsTypeRef) ? (TypeRef) x.Interface : x.Interface.ToTypeRef(),
                        (RoslynAssembly) _ts.FindAssembly(x.Interface.ResolveTypeDef().DefinitionAssembly.FullName));
                })
                .ToList();

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            if (TypeRef is TypeDef || TypeRef is TypeRef)
            {
                var typeSig = TypeRef.ToTypeSig();

                var sig = new GenericInstSig(typeSig.ToClassOrValueTypeSig(), typeArguments
                    .Select(x => ((RoslynType) x).TypeRef.ToTypeSig())
                    .ToArray());

                var generic = new TypeSpecUser(sig);

                return new RoslynType(_ts, TypeDef, generic, _assembly);
            }

            throw new Exception("Can't create generic Type");
        }

        public IType GenericTypeDefinition { get; }

        public bool IsArray => TypeRef.ToTypeSig().IsSingleOrMultiDimensionalArray;

        public IType ArrayElementType => _ts.Resolve(TypeRef.ScopeType);

        public IType MakeArrayType()
        {
            return new RoslynType(_ts, TypeDef, new TypeSpecUser(new SZArraySig(TypeRef.ToTypeSig())), _assembly);
        }

        public IType MakeArrayType(int dimensions)
        {
            return new RoslynType(_ts, TypeDef, new TypeSpecUser(new ArraySig(TypeRef.ToTypeSig(), dimensions)),
                _assembly);
        }

        public IType BaseType => (TypeDef.BaseType != null) ? _ts.Resolve(TypeDef.BaseType) : null;

        public bool IsValueType => TypeDef.IsValueType;

        public bool IsEnum => TypeDef.IsEnum;

        public bool IsInterface => TypeDef.IsInterface;

        public bool IsSystem => false;

        public bool IsPrimitive => TypeDef.IsPrimitive;

        public bool IsPublic => TypeDef.IsPublic;

        public bool IsAbstract => TypeDef.IsAbstract;

        public bool IsGeneric => TypeRef.ToTypeSig() is GenericInstSig;

        public IType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }

        public virtual void DumpRef(TextWriter tw)
        {
            if (FullName == "System.Void")
            {
                tw.W("void");
                return;
            }

            if (IsArray)
            {
            }

            if (IsGeneric)
            {
                tw.Write(GetNameWithoutGenericArity(FullName));
                using (tw.AngleBrace())
                {
                    var wasFirst = false;
                    foreach (var arg in GenericArguments)
                    {
                        if (wasFirst)
                            tw.W(",");

                        ((RoslynType) arg).DumpRef(tw);

                        wasFirst = true;
                    }
                }
            }
            else
                tw.Write(FullName);
        }

        private string GetNameWithoutGenericArity(string name)
        {
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        public bool Equals(IType other)
        {
            throw new NotImplementedException();
        }
    }
}