using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    class SynthesizedFieldSymbol : FieldSymbol
    {
        readonly NamedTypeSymbol _containing;
        TypeSymbol _type;
        string _name;
        ConstantValue _const;
        private Accessibility _accessability;
        private bool _isReadOnly;
        private bool _isStatic;
        private ImmutableArray<AttributeData>.Builder _attributes;


        public SynthesizedFieldSymbol(NamedTypeSymbol containing)
        {
            _containing = containing;

            _type = containing.DeclaringCompilation.CoreTypes.Object;
            _attributes = ImmutableArray.CreateBuilder<AttributeData>();
        }

        public override Symbol AssociatedSymbol => null;

        public override Symbol ContainingSymbol => _containing;

        public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override Accessibility DeclaredAccessibility => _accessability;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;

        public override bool IsConst => _const != null;

        public override bool IsReadOnly => _isReadOnly; // .initonly

        public override bool IsStatic => _isStatic;

        public override bool IsVolatile => false;

        public override bool IsImplicitlyDeclared => true;

        public override string Name => _name;

        internal override bool HasRuntimeSpecialName => false;

        internal override bool HasSpecialName => false;

        internal override bool IsNotSerialized => false;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override ImmutableArray<AttributeData> GetAttributes()
        {
            return _attributes.ToImmutable();
        }

        internal override int? TypeLayoutOffset => null;

        internal override ConstantValue GetConstantValue(bool earlyDecodingWellKnownAttributes) => _const;

        internal override TypeSymbol GetFieldType(ConsList<FieldSymbol> fieldsBeingBound) => _type;

        internal SynthesizedFieldSymbol SetType(TypeSymbol type)
        {
            _type = type;
            return this;
        }

        internal SynthesizedFieldSymbol SetAccess(Accessibility value)
        {
            _accessability = value;
            return this;
        }

        internal SynthesizedFieldSymbol SetReadOnly(bool value)
        {
            _isReadOnly = value;
            return this;
        }

        internal SynthesizedFieldSymbol SetIsStatic(bool value)
        {
            _isStatic = value;
            return this;
        }

        internal SynthesizedFieldSymbol SetName(string value)
        {
            _name = value;
            return this;
        }

        internal SynthesizedFieldSymbol SetConstant(ConstantValue value)
        {
            _const = value;
            return this;
        }

        internal SynthesizedFieldSymbol AddAttribute(AttributeData attribute)
        {
            _attributes.Add(attribute);
            return this;
        }
    }
}