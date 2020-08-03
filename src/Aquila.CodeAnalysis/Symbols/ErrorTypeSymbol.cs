﻿﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Diagnostics;
 using Pchp.CodeAnalysis;
 using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// An ErrorSymbol is used when the compiler cannot determine a symbol object to return because
    /// of an error. For example, if a field is declared "Foo x;", and the type "Foo" cannot be
    /// found, an ErrorSymbol is returned when asking the field "x" what it's type is.
    /// </summary>
    internal abstract partial class ErrorTypeSymbol : NamedTypeSymbol, IErrorTypeSymbol
    {
        internal static readonly ErrorTypeSymbol UnknownResultType = new UnsupportedMetadataTypeSymbol();

        public abstract CandidateReason CandidateReason { get; }

        public override string Name => string.Empty;

        public override int Arity => 0;

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) => GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override TypeKind TypeKind => TypeKind.Error;

        public override SymbolKind Kind => SymbolKind.ErrorType;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public virtual ImmutableArray<ISymbol> CandidateSymbols => ImmutableArray<ISymbol>.Empty;

        /// <summary>
        /// Called by <see cref="AbstractTypeMap.SubstituteType"/> to perform substitution
        /// on types with TypeKind ErrorType.  The general pattern is to use the type map
        /// to perform substitution on the wrapped type, if any, and then construct a new
        /// error type symbol from the result (if there was a change).
        /// </summary>
        internal virtual TypeWithModifiers Substitute(AbstractTypeMap typeMap)
        {
            return new TypeWithModifiers((ErrorTypeSymbol)typeMap.SubstituteNamedType(this));
        }

        internal override bool MangleName => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override bool ShouldAddWinRTMembers => false;

        internal override TypeLayout Layout => default(TypeLayout);

        public override Symbol ContainingSymbol => null;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsSerializable => false;

        internal override bool IsInterface => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override IEnumerable<IFieldSymbol> GetFieldsToEmit()
        {
            yield break;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers() => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembers(string name) => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<Symbol> GetMembersByPhpName(string name) => ImmutableArray<Symbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;
    }

    internal sealed class UnsupportedMetadataTypeSymbol : ErrorTypeSymbol
    {
        readonly BadImageFormatException _mrEx;

        internal UnsupportedMetadataTypeSymbol(BadImageFormatException mrEx = null)
        {
            _mrEx = mrEx;
        }

        public override CandidateReason CandidateReason => CandidateReason.None;

        internal override bool MangleName
        {
            get
            {
                return false;
            }
        }
    }

    internal class MissingMetadataTypeSymbol : ErrorTypeSymbol
    {
        /// <summary>
        /// Represents nested missing type.
        /// </summary>
        internal class Nested : MissingMetadataTypeSymbol
        {
            private readonly NamedTypeSymbol _containingType;

            public Nested(NamedTypeSymbol containingType, string name, int arity, bool mangleName)
                : base(name, arity, mangleName)
            {
                Debug.Assert((object)containingType != null);

                _containingType = containingType;
            }

            public Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName)
                : this(containingType, ref emittedName, emittedName.ForcedArity == -1 || emittedName.ForcedArity == emittedName.InferredArity)
            {
            }

            private Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName, bool mangleName)
                : this(containingType,
                       mangleName ? emittedName.UnmangledTypeName : emittedName.TypeName,
                       mangleName ? emittedName.InferredArity : emittedName.ForcedArity,
                       mangleName)
            {
            }

            public override Symbol ContainingSymbol
            {
                get
                {
                    return _containingType;
                }
            }


            public override SpecialType SpecialType
            {
                get
                {
                    return SpecialType.None; // do not have nested types among CORE types yet.
                }
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_containingType, Hash.Combine(MetadataName, _arity));
            }

            internal override bool Equals(TypeSymbol t2, bool ignoreCustomModifiersAndArraySizesAndLowerBounds = false, bool ignoreDynamic = false)
            {
                if (ReferenceEquals(this, t2))
                {
                    return true;
                }

                var other = t2 as Nested;
                return (object)other != null && string.Equals(MetadataName, other.MetadataName, StringComparison.Ordinal) &&
                    _arity == other._arity &&
                    _containingType.Equals(other._containingType, ignoreCustomModifiersAndArraySizesAndLowerBounds, ignoreDynamic);
            }
        }

        protected readonly string _name;
        protected readonly int _arity;
        protected readonly bool _mangleName;

        public MissingMetadataTypeSymbol(string name, int arity, bool mangleName)
        {
            Debug.Assert(name != null);

            this._name = name;
            this._arity = arity;
            this._mangleName = (mangleName && arity > 0);

        }
        public override CandidateReason CandidateReason => CandidateReason.None;

        public override string Name => _name;
        internal override bool MangleName => _mangleName;
        public override int Arity => _arity;
    }

    internal sealed class AmbiguousErrorTypeSymbol : ErrorTypeSymbol
    {
        internal ImmutableArray<NamedTypeSymbol> _candidates;

        public AmbiguousErrorTypeSymbol(ImmutableArray<NamedTypeSymbol> candidates)
        {
            Debug.Assert(!candidates.IsDefaultOrEmpty);
            _candidates = candidates;
        }

        public override CandidateReason CandidateReason => CandidateReason.Ambiguous;
        public override ImmutableArray<ISymbol> CandidateSymbols => _candidates.CastArray<ISymbol>();

        public override string Name => _candidates[0].Name;
        internal override bool MangleName => _candidates[0].MangleName;
        public override int Arity => 0;
    }
}
