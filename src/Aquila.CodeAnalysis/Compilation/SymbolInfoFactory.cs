// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    internal static class SymbolInfoFactory
    {
        internal static SymbolInfo Create(ImmutableArray<Symbol> symbols, bool isDynamic)
        {
            if (isDynamic)
            {
                if (symbols.Length == 1)
                {
                    return new SymbolInfo(symbols[0], CandidateReason.LateBound);
                }
                else
                {
                    return new SymbolInfo(symbols
                        .Cast<ISymbol>().ToImmutableArray(), CandidateReason.LateBound);
                }
            }
            else
            {
                return new SymbolInfo(symbols.Cast<ISymbol>().ToImmutableArray(), CandidateReason.None);
            }
        }
    }
}