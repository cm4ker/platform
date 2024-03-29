﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    #region SynthesizedDtorSymbol // generated Finalize method

    internal class SynthesizedFinalizeSymbol : SynthesizedMethodSymbol
    {
        public SynthesizedFinalizeSymbol(NamedTypeSymbol container)
            : base(container, WellKnownMemberNames.DestructorName, false, true,
                container.DeclaringCompilation.CoreTypes.Void, aquilahidden: true)
        {
            Debug.Assert(!container.IsStatic);

            SetOverride((MethodSymbol)container.DeclaringCompilation
                .GetSpecialType(SpecialType.System_Object)
                .GetMembers(WellKnownMemberNames.DestructorName)
                .Single());
        }

        public override bool HidesBaseMethodsByName => false;

        internal override bool HasSpecialName => false;

        public override Accessibility DeclaredAccessibility => Accessibility.Protected;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsExtern => false;

        public sealed override bool IsSealed => false;

        public sealed override MethodKind MethodKind => MethodKind.Destructor;

        internal override bool IsExplicitInterfaceImplementation => true;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false) => false;

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false) => true;

        internal override bool IsMetadataFinal => false;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<Location> Locations
        {
            get { throw new NotImplementedException(); }
        }

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;
    }

    #endregion
}