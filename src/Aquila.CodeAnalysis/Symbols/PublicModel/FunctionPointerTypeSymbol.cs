﻿﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable

 using System.Diagnostics;
 using System.Diagnostics.CodeAnalysis;
 using Microsoft.CodeAnalysis;
 using Roslyn.Utilities;

 namespace Aquila.CodeAnalysis.Symbols.PublicModel
{
    internal sealed class FunctionPointerTypeSymbol : TypeSymbol, IFunctionPointerTypeSymbol
    {
        private readonly FunctionPointers.FunctionPointerTypeSymbol _underlying;

        public FunctionPointerTypeSymbol(FunctionPointers.FunctionPointerTypeSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            RoslynDebug.Assert(underlying is object);
            _underlying = underlying;
        }

        public IMethodSymbol Signature => _underlying.Signature.GetPublicSymbol();
        internal override Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;
        internal override Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;
        internal override Symbols.Symbol UnderlyingSymbol => _underlying;

        protected override void Accept(SymbolVisitor visitor)
            => visitor.VisitFunctionPointerType(this);

        [return: MaybeNull]
        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerType(this);
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            Debug.Assert(nullableAnnotation != this.NullableAnnotation);
            Debug.Assert(nullableAnnotation != _underlying.DefaultNullableAnnotation);
            return new FunctionPointerTypeSymbol(_underlying, nullableAnnotation);
        }
    }
}
