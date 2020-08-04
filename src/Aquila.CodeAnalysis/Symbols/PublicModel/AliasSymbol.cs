﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

 using System.Diagnostics.CodeAnalysis;
 using Microsoft.CodeAnalysis;
 using Roslyn.Utilities;

 namespace Aquila.CodeAnalysis.Symbols.PublicModel
{
    internal sealed class AliasSymbol : Symbol, IAliasSymbol
    {
        private readonly Symbols.AliasSymbol _underlying;

        public AliasSymbol(Symbols.AliasSymbol underlying)
        {
            RoslynDebug.Assert(underlying is object);
            _underlying = underlying;
        }

        internal override Symbols.Symbol UnderlyingSymbol => _underlying;

        INamespaceOrTypeSymbol IAliasSymbol.Target
        {
            get
            {
                return _underlying.Target.GetPublicSymbol();
            }
        }

        #region ISymbol Members

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitAlias(this);
        }

        [return: MaybeNull]
        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitAlias(this);
        }

        #endregion
    }
}
