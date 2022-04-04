// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents a label in method body
    /// </summary>
    internal abstract class LabelSymbol : Symbol
    {
        /// <summary>
        /// Returns false because label can't be defined externally.
        /// </summary>
        public override bool IsExtern
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false because label can't be sealed.
        /// </summary>
        public override bool IsSealed
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false because label can't be abstract.
        /// </summary>
        public override bool IsAbstract
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false because label can't be overridden.
        /// </summary>
        public override bool IsOverride
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false because label can't be virtual.
        /// </summary>
        public override bool IsVirtual
        {
            get { return false; }
        }

        /// <summary>
        /// Returns false because label can't be static.
        /// </summary>
        public override bool IsStatic
        {
            get { return false; }
        }

        /// <summary>
        /// Returns data decoded from Obsolete attribute or null if there is no Obsolete attribute.
        /// This property returns ObsoleteAttributeData.Uninitialized if attribute arguments haven't been decoded yet.
        /// </summary>
        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData
        {
            get { return null; }
        }

        /// <summary>
        /// Returns 'NotApplicable' because label can't be used outside the member body.
        /// </summary>
        public override Accessibility DeclaredAccessibility
        {
            get { return Accessibility.NotApplicable; }
        }

        /// <summary>
        /// Gets the locations where the symbol was originally defined, either in source or
        /// metadata. Some symbols (for example, partial classes) may be defined in more than one
        /// location.
        /// </summary>
        public override ImmutableArray<Location> Locations
        {
            get { throw new NotSupportedException(); }
        }

        internal virtual SyntaxNodeOrToken IdentifierNodeOrToken
        {
            get { return default(SyntaxNodeOrToken); }
        }

        /// <summary>
        /// Gets the immediately containing symbol of the <see cref="LabelSymbol"/>.
        /// It should be the <see cref="MethodSymbol"/> containing the label in its body.
        /// </summary>
        public virtual MethodSymbol ContainingMethod
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the immediately containing symbol of the <see cref="LabelSymbol"/>.
        /// It should be the <see cref="MethodSymbol"/> containing the label in its body.
        /// </summary>
        public override Symbol ContainingSymbol
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Returns value 'Label' of the <see cref="SymbolKind"/>
        /// </summary>
        public override SymbolKind Kind
        {
            get { return SymbolKind.Label; }
        }
    }
}