using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    internal class SynthesizedPropertySymbol : PropertySymbol
    {
        readonly NamedTypeSymbol _containing;

        Accessibility _accessibility;
        MethodSymbol _setMethod, _getMethod;
        string _name;
        bool _isStatic;
        TypeSymbol _type;

        public SynthesizedPropertySymbol(NamedTypeSymbol containing)
        {
            _containing = containing;
            _type = DeclaringCompilation.CoreTypes.Object;
        }

        public override Symbol ContainingSymbol => _containing;

        public override Accessibility DeclaredAccessibility => _accessibility;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences =>
            ImmutableArray<SyntaxReference>.Empty;

        public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
        {
            get { return ImmutableArray<PropertySymbol>.Empty; }
        }

        public override MethodSymbol GetMethod => _getMethod;

        public override MethodSymbol SetMethod => _setMethod;

        public override bool IsAbstract => false;

        public override bool IsExtern => false;

        public override bool IsIndexer => false;

        public override bool IsOverride => false;

        public override bool IsSealed => false;

        public override bool IsStatic => _isStatic;

        public override bool IsVirtual => !IsStatic && (!IsSealed || IsOverride);

        public override string Name => _name;

        public override ImmutableArray<ParameterSymbol> Parameters { get; } = ImmutableArray<ParameterSymbol>.Empty;

        public override TypeSymbol Type => _type;

        public override ImmutableArray<CustomModifier> TypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        internal override bool HasSpecialName => false;

        internal override bool MustCallMethodsDirectly => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        #region Build API

        public SynthesizedPropertySymbol SetName(string value)
        {
            _name = value;
            return this;
        }

        public SynthesizedPropertySymbol SetAccess(Accessibility value)
        {
            _accessibility = value;
            return this;
        }

        public SynthesizedPropertySymbol SetSetMethod(MethodSymbol value)
        {
            _setMethod = value;
            return this;
        }

        public SynthesizedPropertySymbol SetGetMethod(MethodSymbol value)
        {
            _getMethod = value;
            return this;
        }

        public SynthesizedPropertySymbol SetType(TypeSymbol value)
        {
            _type = value;
            return this;
        }

        public SynthesizedPropertySymbol SetIsStatic(bool value)
        {
            _isStatic = value;
            return this;
        }

        #endregion
    }
}