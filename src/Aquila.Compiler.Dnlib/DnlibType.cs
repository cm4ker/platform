using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using Aquila.Compiler.Contracts;
using IAssembly = Aquila.Compiler.Contracts.IAssembly;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IField = Aquila.Compiler.Contracts.IField;
using IMethod = Aquila.Compiler.Contracts.IMethod;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class DnlibType : IType
    {
        private readonly DnlibTypeSystem _ts;
        private readonly DnlibAssembly _assembly;
        private readonly DnlibContextResolver _cr;

        public DnlibType(DnlibTypeSystem typeSystem, TypeDef typeDef, ITypeDefOrRef typeRef, DnlibAssembly assembly)
        {
            _ts = typeSystem;
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            TypeRef = typeRef ?? throw new ArgumentNullException(nameof(typeRef));
            TypeDef = typeDef ?? typeRef.ResolveTypeDef();

            if (typeDef is null) throw new ArgumentNullException(nameof(typeDef));
            _cr = new DnlibContextResolver(_ts, typeDef.Module);

            Id = TypeConstants.GetIdFromName(FullName);
        }

        public TypeDef TypeDef { get; }

        public ITypeDefOrRef TypeRef { get; }

        public bool Equals(IType other)
        {
            return new SigComparer().Equals(TypeRef, ((DnlibType) other)?.TypeRef);
        }

        public ITypeSystem TypeSystem => _ts;


        public Guid Id { get; }

        public string Name => TypeDef.Name;
        public string Namespace => TypeDef.Namespace;
        public string FullName => TypeDef.FullName;
        public IAssembly Assembly => _assembly;

        private IReadOnlyList<IProperty> _properties;
        private IReadOnlyList<IField> _fields;
        private IReadOnlyList<IMethod> _methods;
        private IReadOnlyList<IConstructor> _constructors;
        private IReadOnlyList<IType> _interfaces;
        private IReadOnlyList<IType> _genericArguments;
        private IReadOnlyList<ICustomAttribute> _customAttributes;

        public IReadOnlyList<IProperty> Properties =>
            _properties ??= TypeDef.Properties.Select(x => new DnlibProperty(_ts, x, TypeRef)).ToList();

        public IReadOnlyList<IField> Fields =>
            _fields ??= TypeDef.Fields.Select(x => new DnlibField(_ts, x)).ToList();

        public IReadOnlyList<IEventInfo> Events { get; }

        public IReadOnlyList<IMethod> Methods =>
            _methods ??= CalculateMethods();

        public IMethod CalculateMethod(MethodDef x)
        {
            return new DnlibMethod(_ts,
                new MemberRefUser(x.Module, x.Name, _cr.ResolveMethodSig(x.MethodSig, GenericArguments?.ToArray()),
                    TypeRef),
                x, TypeRef);
        }

        private List<IMethod> CalculateMethods()
        {
            return TypeDef.Methods.Where(x => !x.IsConstructor)
                .Select(CalculateMethod)
                .ToList();
        }

        public IReadOnlyList<IConstructor> Constructors =>
            _constructors ??= TypeDef.FindConstructors().Select(x =>
            {
                return (IConstructor) new DnlibConstructor(_ts,
                    new MemberRefUser(x.Module, x.Name, x.MethodSig, TypeRef), x,
                    TypeRef);
            }).ToList();

        public IReadOnlyList<ICustomAttribute> CustomAttributes
            => _customAttributes ??= new List<ICustomAttribute>();

        public IReadOnlyList<IType> GenericArguments =>
            _genericArguments ??= (TypeRef as TypeSpec)?.TryGetGenericInstSig()?.GenericArguments?
                .Select(x => _cr.GetType(x)).ToList();

        public IReadOnlyList<IType> Interfaces =>
            _interfaces ??= TypeDef.Interfaces
                .Where(x => x.Interface.ResolveTypeDef() != null).Select(x =>
                {
                    return new DnlibType(_ts, x.Interface.ResolveTypeDef(),
                        (x.Interface.IsTypeRef) ? (TypeRef) x.Interface : x.Interface.ToTypeRef(),
                        (DnlibAssembly) _ts.FindAssembly(x.Interface.ResolveTypeDef().DefinitionAssembly.FullName));
                })
                .ToList();

        public IType MakeGenericType(IReadOnlyList<IType> typeArguments)
        {
            if (TypeRef is TypeDef || TypeRef is TypeRef)
            {
                var typeSig = TypeRef.ToTypeSig();

                var sig = new GenericInstSig(typeSig.ToClassOrValueTypeSig(), typeArguments
                    .Select(x => ((DnlibType) x).TypeRef.ToTypeSig())
                    .ToArray());

                var generic = new TypeSpecUser(sig);

                return new DnlibType(_ts, TypeDef, generic, _assembly);
            }

            throw new Exception("Can't create generic Type");
        }

        public IType GenericTypeDefinition { get; }

        public bool IsArray => TypeRef.ToTypeSig().IsSingleOrMultiDimensionalArray;

        public IType ArrayElementType => _ts.Resolve(TypeRef.ScopeType);

        public IType MakeArrayType()
        {
            return new DnlibType(_ts, TypeDef, new TypeSpecUser(new SZArraySig(TypeRef.ToTypeSig())), _assembly);
        }

        public IType MakeArrayType(int dimensions)
        {
            return new DnlibType(_ts, TypeDef, new TypeSpecUser(new ArraySig(TypeRef.ToTypeSig(), dimensions)),
                _assembly);
        }

        public IType BaseType => (TypeDef.BaseType != null) ? _ts.Resolve(TypeDef.BaseType) : null;

        public bool IsValueType => TypeDef.IsValueType;

        public bool IsEnum => TypeDef.IsEnum;

        public bool IsInterface => TypeDef.IsInterface;

        public bool IsSystem => false;

        public bool IsPrimitive => TypeDef.IsPrimitive;

        public IType GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }
    }
}