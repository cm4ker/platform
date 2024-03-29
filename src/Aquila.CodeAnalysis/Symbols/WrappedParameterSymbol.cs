﻿using System.Collections.Immutable;
using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    internal abstract class WrappedParameterSymbol : ParameterSymbol
    {
        protected readonly ParameterSymbol underlyingParameter;

        protected WrappedParameterSymbol(ParameterSymbol underlyingParameter)
        {
            Debug.Assert((object)underlyingParameter != null);

            this.underlyingParameter = underlyingParameter;
        }

        public abstract override Symbol ContainingSymbol
        {
            get;
        }

        protected override Symbol OriginalSymbolDefinition => this;

        public sealed override bool Equals(ISymbol obj, SymbolEqualityComparer equalityComparer)
        {
            if ((object)this == obj)
            {
                return true;
            }

            // Equality of ordinal and containing symbol is a correct
            // implementation for all ParameterSymbols, but we don't 
            // define it on the base type because most can simply use
            // ReferenceEquals.

            var other = obj as WrappedParameterSymbol;
            return (object)other != null &&
                this.Ordinal == other.Ordinal &&
                SymbolEqualityComparer.Default.Equals(this.ContainingSymbol, other.ContainingSymbol);
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(ContainingSymbol, underlyingParameter.Ordinal);
        }

        #region Forwarded

        public override BoundExpression Initializer => underlyingParameter.Initializer;

        public override FieldSymbol DefaultValueField => underlyingParameter.DefaultValueField;

        internal override TypeSymbol Type => underlyingParameter.Type;

        public sealed override RefKind RefKind => underlyingParameter.RefKind;
        
        public sealed override ImmutableArray<Location> Locations
        {
            get { return underlyingParameter.Locations; }
        }

        public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return underlyingParameter.DeclaringSyntaxReferences; }
        }

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            return underlyingParameter.GetAttributes();
        }

        internal sealed override ConstantValue ExplicitDefaultConstantValue
        {
            get { return underlyingParameter.ExplicitDefaultConstantValue; }
        }

        public override int Ordinal
        {
            get { return underlyingParameter.Ordinal; }
        }

        public override bool IsParams
        {
            get { return underlyingParameter.IsParams; }
        }

        public override bool IsImplicitlyDeclared
        {
            get { return underlyingParameter.IsImplicitlyDeclared; }
        }

        public sealed override string Name
        {
            get { return underlyingParameter.Name; }
        }

        public sealed override string MetadataName
        {
            get { return underlyingParameter.MetadataName; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get { return underlyingParameter.CustomModifiers; }
        }

        internal override ImportValueAttributeData ImportValueAttributeData => underlyingParameter.ImportValueAttributeData;

        public override bool HasNotNull => underlyingParameter.HasNotNull;

        #endregion
    }
}
