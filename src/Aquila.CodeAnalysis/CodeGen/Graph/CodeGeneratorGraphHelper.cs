using System;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;

namespace Aquila.CodeAnalysis.Semantics;

internal static class CodeGeneratorGraphHelper
{
    internal static SourceViewTypeSymbol.MethodTreeBuilderSymbol GetOrThrowViewMethod(this SourceMethodSymbolBase method)
    {
        if (method is not SourceViewTypeSymbol.MethodTreeBuilderSymbol s)
        {
            throw new InvalidOperationException(
                $"Only {nameof(SourceViewTypeSymbol.MethodTreeBuilderSymbol)} can be a host for this instruction");
        }

        return s;
    }
}