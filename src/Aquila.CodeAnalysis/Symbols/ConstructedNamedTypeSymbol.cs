﻿using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// A named type symbol that results from substituting a new owner for a type declaration.
    /// </summary>
    internal sealed class SubstitutedNestedTypeSymbol : SubstitutedNamedTypeSymbol
    {
        internal SubstitutedNestedTypeSymbol(SubstitutedNamedTypeSymbol newContainer, NamedTypeSymbol originalDefinition)
            : base(
                newContainer: newContainer,
                map: newContainer.TypeSubstitution,
                originalDefinition: originalDefinition,
                // An Arity-0 member of an unbound type, e.g. A<>.B, is unbound.
                unbound: newContainer.IsUnboundGenericType && originalDefinition.Arity == 0)
        {
        }

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get
            {
                return TypeParameters.CastArray<TypeSymbol>();
            }
        }

        internal override bool HasTypeArgumentsCustomModifiers => false;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal) => GetEmptyTypeArgumentCustomModifiers(ordinal);

        public override NamedTypeSymbol ConstructedFrom
        {
            get { return this; }
        }
    }

    /// <summary>
    /// A generic named type symbol that has been constructed with type arguments distinct from its own type parameters.
    /// </summary>
    internal sealed class ConstructedNamedTypeSymbol : SubstitutedNamedTypeSymbol
    {
        private readonly ImmutableArray<TypeSymbol> _typeArguments;
        private readonly bool _hasTypeArgumentsCustomModifiers;
        private readonly NamedTypeSymbol _constructedFrom;

        internal ConstructedNamedTypeSymbol(NamedTypeSymbol constructedFrom, ImmutableArray<TypeWithModifiers> typeArguments, bool unbound = false)
            : base(newContainer: constructedFrom.ContainingSymbol,
                   map: new TypeMap(constructedFrom.ContainingType, constructedFrom.OriginalDefinition.TypeParameters, typeArguments),
                   originalDefinition: constructedFrom.OriginalDefinition,
                   constructedFrom: constructedFrom, unbound: unbound)
        {
            bool hasTypeArgumentsCustomModifiers = false;
            _typeArguments = typeArguments.SelectAsArray(a =>
                                                            {
                                                                if (!a.CustomModifiers.IsDefaultOrEmpty)
                                                                {
                                                                    hasTypeArgumentsCustomModifiers = true;
                                                                }

                                                                return a.Type;
                                                            });
            _hasTypeArgumentsCustomModifiers = hasTypeArgumentsCustomModifiers;
            _constructedFrom = constructedFrom;

            Debug.Assert(constructedFrom.Arity == typeArguments.Length);
            Debug.Assert(constructedFrom.Arity != 0);
        }

        public override NamedTypeSymbol ConstructedFrom
        {
            get
            {
                return _constructedFrom;
            }
        }

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get
            {
                return _typeArguments;
            }
        }

        internal override bool HasTypeArgumentsCustomModifiers => _hasTypeArgumentsCustomModifiers;

        public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
        {
            if (_hasTypeArgumentsCustomModifiers)
            {
                return TypeSubstitution.GetTypeArgumentsCustomModifiersFor(_constructedFrom.OriginalDefinition.TypeParameters[ordinal]);
            }

            return GetEmptyTypeArgumentCustomModifiers(ordinal);
        }

        internal static bool TypeParametersMatchTypeArguments(ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeWithModifiers> typeArguments)
        {
            int n = typeParameters.Length;
            Debug.Assert(typeArguments.Length == n);
            Debug.Assert(typeArguments.Length > 0);

            for (int i = 0; i < n; i++)
            {
                if (!typeArguments[i].Is(typeParameters[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
