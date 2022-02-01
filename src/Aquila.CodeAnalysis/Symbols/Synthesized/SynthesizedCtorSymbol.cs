using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    #region SynthesizedCtorSymbol

    internal class SynthesizedCtorSymbol : SynthesizedMethodSymbol
    {
        public SynthesizedCtorSymbol(NamedTypeSymbol container)
            : base(container, WellKnownMemberNames.InstanceConstructorName, false, false,
                container.DeclaringCompilation.CoreTypes.Void)
        {
            Debug.Assert(!container.IsStatic);
        }

        public override bool HidesBaseMethodsByName => false;

        internal override bool HasSpecialName => true;

        public override Accessibility DeclaredAccessibility => Accessibility.Public;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsExtern => false;

        public sealed override bool IsOverride => false;

        public sealed override bool IsSealed => false;

        public sealed override bool IsVirtual => false;

        public sealed override MethodKind MethodKind => MethodKind.Constructor;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false) => false;

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false) => false;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;
    }

    #endregion
}