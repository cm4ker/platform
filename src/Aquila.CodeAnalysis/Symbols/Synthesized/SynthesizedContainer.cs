using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Anonymous;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// A container synthesized for a lambda, iterator method, async method, or dynamic-sites.
    /// </summary>
    internal abstract class SynthesizedContainer : NamedTypeSymbol
    {
        private readonly string _name;
        private readonly TypeMap _typeMap;
        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

        protected SynthesizedContainer(string name, int parameterCount, bool returnsVoid)
        {
            Debug.Assert(name != null);
            _name = name;
            _typeMap = TypeMap.Empty;
            _typeParameters = CreateTypeParameters(parameterCount, returnsVoid);
        }

        protected SynthesizedContainer(string name, MethodSymbol topLevelMethod)
        {
            Debug.Assert(name != null);
            Debug.Assert(topLevelMethod != null);

            _name = name;
            _typeMap = TypeMap.Empty.WithAlphaRename(topLevelMethod, this, out _typeParameters);
        }

        protected SynthesizedContainer(string name, ImmutableArray<TypeParameterSymbol> typeParameters, TypeMap typeMap)
        {
            Debug.Assert(name != null);
            Debug.Assert(!typeParameters.IsDefault);
            Debug.Assert(typeMap != null);

            _name = name;
            _typeParameters = typeParameters;
            _typeMap = typeMap;
        }

        private ImmutableArray<TypeParameterSymbol> CreateTypeParameters(int parameterCount, bool returnsVoid)
        {
            var typeParameters = ArrayBuilder<TypeParameterSymbol>.GetInstance(parameterCount + (returnsVoid ? 0 : 1));
            if (parameterCount != 0)
            {
                for (int i = 0; i < parameterCount; i++)
                {
                    typeParameters.Add(new AnonymousTypeParameterSymbol(this, i, "T" + (i + 1)));
                }
            }

            if (!returnsVoid)
            {
                typeParameters.Add(new AnonymousTypeParameterSymbol(this, parameterCount, "TResult"));
            }

            return typeParameters.ToImmutableAndFree();
        }

        internal TypeMap TypeMap
        {
            get { return _typeMap; }
        }

        internal virtual MethodSymbol Constructor
        {
            get { return null; }
        }

        internal sealed override bool IsInterface
        {
            get { return this.TypeKind == TypeKind.Interface; }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return _typeParameters; }
        }

        public sealed override string Name
        {
            get { return _name; }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return ImmutableArray<SyntaxReference>.Empty; }
        }

        public override IEnumerable<string> MemberNames
        {
            get { return SpecializedCollections.EmptyEnumerable<string>(); }
        }

        public override NamedTypeSymbol ConstructedFrom
        {
            get { return this; }
        }

        public override bool IsSealed
        {
            get { return true; }
        }

        public override bool IsAbstract
        {
            get { return (object)Constructor == null; }
        }

        public override bool IsSerializable => false;

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get { return StaticCast<TypeSymbol>.From(TypeParameters); }
        }

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) =>
            GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override ImmutableArray<Symbol> GetMembers()
        {
            Symbol constructor = this.Constructor;
            return (object)constructor == null ? ImmutableArray<Symbol>.Empty : ImmutableArray.Create(constructor);
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            var ctor = Constructor;
            return ((object)ctor != null && name == ctor.Name)
                ? ImmutableArray.Create<Symbol>(ctor)
                : ImmutableArray<Symbol>.Empty;
        }


        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            foreach (var m in this.GetMembers())
            {
                switch (m.Kind)
                {
                    case SymbolKind.Field:
                        yield return (FieldSymbol)m;
                        break;
                }
            }
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override Accessibility DeclaredAccessibility
        {
            get { return Accessibility.Private; }
        }

        public override bool IsStatic
        {
            get { return false; }
        }

        public override ImmutableArray<NamedTypeSymbol> Interfaces
        {
            get { return ImmutableArray<NamedTypeSymbol>.Empty; }
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return CalculateInterfacesToEmit();
        }

        public override NamedTypeSymbol BaseType //NoUseSiteDiagnostics
        {
            get
            {
                return ContainingAssembly.GetSpecialType(this.TypeKind == TypeKind.Struct
                    ? SpecialType.System_ValueType
                    : SpecialType.System_Object);
            }
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
        {
            return Interfaces;
        }

        public override bool MightContainExtensionMethods
        {
            get { return false; }
        }

        public override int Arity
        {
            get { return TypeParameters.Length; }
        }

        internal override bool MangleName
        {
            get { return Arity > 0; }
        }

        public override bool IsImplicitlyDeclared
        {
            get { return true; }
        }

        internal override bool ShouldAddWinRTMembers
        {
            get { return false; }
        }

        internal override bool IsWindowsRuntimeImport
        {
            get { return false; }
        }

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
        }

        internal override TypeLayout Layout
        {
            get { return default(TypeLayout); }
        }

    }
}