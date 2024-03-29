﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    internal sealed class SubstitutedFieldSymbol : FieldSymbol
    {
        NamedTypeSymbol _containingType;
        readonly FieldSymbol _originalDefinition;
        readonly object _token;

        private TypeSymbol _lazyType;

        internal SubstitutedFieldSymbol(NamedTypeSymbol containingType, FieldSymbol substitutedFrom)
            : this(containingType, substitutedFrom, containingType)
        {
        }

        internal SubstitutedFieldSymbol(NamedTypeSymbol containingType, FieldSymbol substitutedFrom, object token)
        {
            _containingType = containingType;
            _originalDefinition = substitutedFrom.OriginalDefinition as FieldSymbol;
            _token = token ?? _containingType;
        }

        internal override TypeSymbol GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            if ((object)_lazyType == null)
            {
                Interlocked.CompareExchange(ref _lazyType,
                    _containingType.TypeSubstitution.SubstituteType(_originalDefinition.GetFieldType(fieldsBeingBound))
                        .Type, null);
            }

            return _lazyType;
        }

        public override string Name
        {
            get { return _originalDefinition.Name; }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null,
            bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
        }

        internal override bool HasSpecialName
        {
            get { return _originalDefinition.HasSpecialName; }
        }

        internal override bool HasRuntimeSpecialName
        {
            get { return _originalDefinition.HasRuntimeSpecialName; }
        }

        internal override bool IsNotSerialized
        {
            get { return _originalDefinition.IsNotSerialized; }
        }

        internal override int? TypeLayoutOffset
        {
            get { return _originalDefinition.TypeLayoutOffset; }
        }

        public override Symbol ContainingSymbol
        {
            get { return _containingType; }
        }

        public override NamedTypeSymbol ContainingType
        {
            get { return _containingType; }
        }

        internal void SetContainingType(SubstitutedNamedTypeSymbol type)
        {
            Debug.Assert(_lazyType == null);

            _lazyType = null;
            _containingType = type;
        }

        public override FieldSymbol OriginalDefinition
        {
            get { return _originalDefinition.OriginalDefinition; }
        }

        public override ImmutableArray<Location> Locations
        {
            get { return _originalDefinition.Locations; }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return _originalDefinition.DeclaringSyntaxReferences; }
        }

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            return _originalDefinition.GetAttributes();
        }

        public override Symbol AssociatedSymbol
        {
            get
            {
                Symbol underlying = _originalDefinition.AssociatedSymbol;

                if ((object)underlying == null)
                {
                    return null;
                }

                return underlying.SymbolAsMember(ContainingType);
            }
        }

        public override bool IsStatic
        {
            get { return _originalDefinition.IsStatic; }
        }

        public override bool IsReadOnly
        {
            get { return _originalDefinition.IsReadOnly; }
        }

        public override bool IsConst
        {
            get { return _originalDefinition.IsConst; }
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return _originalDefinition.ObsoleteAttributeData; }
        }

        public override object ConstantValue
        {
            get { return _originalDefinition.ConstantValue; }
        }

        internal override ConstantValue GetConstantValue(bool earlyDecodingWellKnownAttributes)
        {
            return _originalDefinition.GetConstantValue(earlyDecodingWellKnownAttributes);
        }

        internal override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get { return _originalDefinition.MarshallingInformation; }
        }

        public override bool IsVolatile
        {
            get { return _originalDefinition.IsVolatile; }
        }

        public override bool IsImplicitlyDeclared
        {
            get { return _originalDefinition.IsImplicitlyDeclared; }
        }

        public override Accessibility DeclaredAccessibility
        {
            get { return _originalDefinition.DeclaredAccessibility; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get
            {
                return _containingType.TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.Type,
                    _originalDefinition.CustomModifiers);
            }
        }

        public override bool Equals(ISymbol other, SymbolEqualityComparer equalityComparer)
        {
            if ((object)this == other)
            {
                return true;
            }

            return other is SubstitutedFieldSymbol f && _token == f._token &&
                   SymbolEqualityComparer.Default.Equals(_originalDefinition, f._originalDefinition);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_token, _originalDefinition.GetHashCode());
        }
    }
}