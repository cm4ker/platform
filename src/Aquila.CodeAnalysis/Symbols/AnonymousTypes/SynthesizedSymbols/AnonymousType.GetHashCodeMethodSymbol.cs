﻿﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

 using System.Collections.Immutable;
 using Microsoft.CodeAnalysis;

 namespace Aquila.CodeAnalysis.Symbols.AnonymousTypes.SynthesizedSymbols
{
    internal sealed partial class AnonymousTypeManager
    {
        /// <summary>
        /// Represents an anonymous type 'GetHashCode' method.
        /// </summary>
        private sealed partial class AnonymousTypeGetHashCodeMethodSymbol : SynthesizedMethodBase
        {
            internal AnonymousTypeGetHashCodeMethodSymbol(NamedTypeSymbol container)
                : base(container, WellKnownMemberNames.ObjectGetHashCode)
            {
            }

            public override MethodKind MethodKind
            {
                get { return MethodKind.Ordinary; }
            }

            public override bool ReturnsVoid
            {
                get { return false; }
            }

            public override RefKind RefKind
            {
                get { return RefKind.None; }
            }

            public override TypeWithAnnotations ReturnTypeWithAnnotations
            {
                get { return TypeWithAnnotations.Create(this.Manager.System_Int32); }
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return ImmutableArray<ParameterSymbol>.Empty; }
            }

            public override bool IsOverride
            {
                get { return true; }
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return true;
            }

            internal override bool IsMetadataFinal
            {
                get
                {
                    return false;
                }
            }
        }
    }
}
