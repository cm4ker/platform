﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

 using Aquila.CodeAnalysis.Symbols.Source;
 using Microsoft.CodeAnalysis;
 using Microsoft.CodeAnalysis.PooledObjects;
 using Roslyn.Utilities;

 namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// Represents __value field of an enum.
    /// </summary>
    internal sealed class SynthesizedEnumValueFieldSymbol : SynthesizedFieldSymbolBase
    {
        public SynthesizedEnumValueFieldSymbol(SourceNamedTypeSymbol containingEnum)
            : base(containingEnum, WellKnownMemberNames.EnumBackingFieldName, isPublic: true, isReadOnly: false, isStatic: false)
        {
        }

        internal override bool SuppressDynamicAttribute
        {
            get { return true; }
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return TypeWithAnnotations.Create(((SourceNamedTypeSymbol)ContainingType).EnumUnderlyingType);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            // no attributes should be emitted
        }
    }
}
