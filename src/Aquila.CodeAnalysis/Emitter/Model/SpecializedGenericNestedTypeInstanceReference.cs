﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;
using NamedTypeSymbol = Aquila.CodeAnalysis.Symbols.NamedTypeSymbol;

namespace Aquila.CodeAnalysis.Emit
{
    /// <summary>
    /// Represents a reference to an instantiation of a generic type nested in an instantiation of another generic type.
    /// e.g. 
    /// A{int}.B{string}
    /// A.B{int}.C.D{string}
    /// </summary>
    internal sealed class SpecializedGenericNestedTypeInstanceReference : SpecializedNestedTypeReference, Cci.IGenericTypeInstanceReference
    {
        public SpecializedGenericNestedTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
            : base(underlyingNamedType)
        {
            Debug.Assert(underlyingNamedType.IsDefinition);
            // Definition doesn't have custom modifiers on type arguments
            //Debug.Assert(!underlyingNamedType.HasTypeArgumentsCustomModifiers);
        }

        public sealed override void Dispatch(Cci.MetadataVisitor visitor)
        {
            visitor.Visit((Cci.IGenericTypeInstanceReference)this);
        }

        ImmutableArray<Cci.ITypeReference> Cci.IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            var builder = ArrayBuilder<Cci.ITypeReference>.GetInstance();
            foreach (TypeSymbol type in UnderlyingNamedType.TypeArguments)//TypeArgumentsNoUseSiteDiagnostics)
            {
                builder.Add(moduleBeingBuilt.Translate(type, syntaxNodeOpt: context.SyntaxNode, diagnostics: context.Diagnostics));
            }

            return builder.ToImmutableAndFree();
        }

        Cci.INamedTypeReference Cci.IGenericTypeInstanceReference.GetGenericType(EmitContext context)
        {
            System.Diagnostics.Debug.Assert(UnderlyingNamedType.OriginalDefinition.IsDefinition);
            return this.UnderlyingNamedType.OriginalDefinition;
        }

        public override Cci.IGenericTypeInstanceReference AsGenericTypeInstanceReference
        {
            get { return this; }
        }

        public override Cci.INamespaceTypeReference AsNamespaceTypeReference
        {
            get { return null; }
        }

        public override Cci.INestedTypeReference AsNestedTypeReference
        {
            get { return this; }
        }

        public override Cci.ISpecializedNestedTypeReference AsSpecializedNestedTypeReference
        {
            get { return null; }
        }
    }
}
