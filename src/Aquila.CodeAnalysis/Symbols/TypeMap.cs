﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Utility class for substituting actual type arguments for formal generic type parameters.
    /// </summary>
    internal sealed class TypeMap : AbstractTypeParameterMap
    {
        public static readonly System.Func<TypeSymbol, TypeWithAnnotations> TypeSymbolAsTypeWithModifiers = t => TypeWithAnnotations.Create(t);

        // Only when the caller passes allowAlpha=true do we tolerate substituted (alpha-renamed) type parameters as keys
        internal TypeMap(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeWithAnnotations> to, bool allowAlpha = false)
            : base(ConstructMapping(from, to))
        {
            // mapping contents are read-only hereafter
            Debug.Assert(allowAlpha || !from.Any(tp => tp is SubstitutedTypeParameterSymbol));
        }

        // Only when the caller passes allowAlpha=true do we tolerate substituted (alpha-renamed) type parameters as keys
        internal TypeMap(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeParameterSymbol> to, bool allowAlpha = false)
            : this(from, Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(to, TypeSymbolAsTypeWithModifiers), allowAlpha)
        {
            // mapping contents are read-only hereafter
        }

        internal TypeMap(SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> mapping)
            : base(new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(mapping, ReferenceEqualityComparer.Instance))
        {
            // mapping contents are read-only hereafter
            Debug.Assert(!mapping.Keys.Any(tp => tp is SubstitutedTypeParameterSymbol));
        }

        private static SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> ForType(NamedTypeSymbol containingType)
        {
            var substituted = containingType as SubstitutedNamedTypeSymbol;
            return (object)substituted != null ?
                new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(substituted.TypeSubstitution.Mapping, ReferenceEqualityComparer.Instance) :
                new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);
        }
        internal TypeMap(NamedTypeSymbol containingType, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeWithAnnotations> typeArguments)
            : base(ForType(containingType))
        {
            for (int i = 0; i < typeParameters.Length; i++)
            {
                TypeParameterSymbol tp = typeParameters[i];
                TypeWithAnnotations ta = typeArguments[i];
                if (!ta.Is(tp))
                {
                    Mapping.Add(tp, ta);
                }
            }
        }

        private static readonly SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> s_emptyDictionary =
            new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);

        private TypeMap()
            : base(s_emptyDictionary)
        {
            Debug.Assert(s_emptyDictionary.IsEmpty());
        }

        private static readonly TypeMap s_emptyTypeMap = new TypeMap();
        public static TypeMap Empty
        {
            get
            {
                Debug.Assert(s_emptyTypeMap.Mapping.IsEmpty());
                return s_emptyTypeMap;
            }
        }

        private TypeMap WithAlphaRename(ImmutableArray<TypeParameterSymbol> oldTypeParameters, Symbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            if (oldTypeParameters.Length == 0)
            {
                newTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
                return this;
            }

            // Note: the below assertion doesn't hold while rewriting async lambdas defined inside generic methods.
            // The async rewriter adds a synthesized struct inside the lambda frame and construct a typemap from
            // the lambda frame's substituted type parameters.
            // Debug.Assert(!oldTypeParameters.Any(tp => tp is SubstitutedTypeParameterSymbol));

            // warning: we expose result to the SubstitutedTypeParameterSymbol constructor, below, even before it's all filled in.
            TypeMap result = new TypeMap(this.Mapping);
            ArrayBuilder<TypeParameterSymbol> newTypeParametersBuilder = ArrayBuilder<TypeParameterSymbol>.GetInstance();

            // The case where it is "synthesized" is when we're creating type parameters for a synthesized (generic)
            // class or method for a lambda appearing in a generic method.
            bool synthesized = !ReferenceEquals(oldTypeParameters[0].ContainingSymbol.OriginalDefinition, newOwner.OriginalDefinition);

            foreach (var tp in oldTypeParameters)
            {
                var newTp = synthesized ?
                    new SynthesizedSubstitutedTypeParameterSymbol(newOwner, result, tp) :
                    new SubstitutedTypeParameterSymbol(newOwner, result, tp);
                result.Mapping.Add(tp, TypeWithAnnotations.Create(newTp));
                newTypeParametersBuilder.Add(newTp);
            }

            newTypeParameters = newTypeParametersBuilder.ToImmutableAndFree();
            return result;
        }

        internal TypeMap WithAlphaRename(NamedTypeSymbol oldOwner, NamedTypeSymbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            Debug.Assert(oldOwner.ConstructedFrom == oldOwner);
            return WithAlphaRename(oldOwner.OriginalDefinition.TypeParameters, newOwner, out newTypeParameters);
        }

        internal TypeMap WithAlphaRename(MethodSymbol oldOwner, Symbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            Debug.Assert(object.ReferenceEquals(oldOwner.ConstructedFrom, oldOwner));
            return WithAlphaRename(oldOwner.OriginalDefinition.TypeParameters, newOwner, out newTypeParameters);
        }

        private static SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> ConstructMapping(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeWithAnnotations> to)
        {
            var mapping = new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);

            Debug.Assert(from.Length == to.Length);

            for (int i = 0; i < from.Length; i++)
            {
                TypeParameterSymbol tp = from[i];
                TypeWithAnnotations ta = to[i];
                if (!ta.Is(tp))
                {
                    mapping.Add(tp, ta);
                }
            }

            return mapping;
        }

        public ImmutableArray<CustomModifier> GetTypeArgumentsCustomModifiersFor(TypeParameterSymbol originalDefinition)
        {
            Debug.Assert((object)originalDefinition != null);
            Debug.Assert(originalDefinition.IsDefinition);
            return SubstituteTypeParameter(originalDefinition).CustomModifiers;
        }
    }
}
