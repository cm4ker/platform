using System;
using System.Collections.Generic;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Lowering;

internal class LambdaRewriter : GraphRewriter
{
    private readonly SourceMethodSymbolBase _method;

    private readonly List<BoundVariable> _variablesAbove = new List<BoundVariable>();
    private readonly NamedTypeSymbol _containingType;

    private LambdaRewriter(SourceMethodSymbolBase method, PEModuleBuilder moduleBuilder)
    {
        _method = method;
        _containingType = _method.ContainingType;
    }

    public override object VisitVariable(BoundVariable x)
    {
        _variablesAbove.Add(x);
        return base.VisitVariable(x);
    }

    public override object VisitFuncEx(BoundFuncEx x)
    {
        x.
    }

    public static void Transform(SourceMethodSymbolBase sourceMethodSymbolBase, PEModuleBuilder moduleBuilder)
    {
        var rewriter = new LambdaRewriter(sourceMethodSymbolBase, moduleBuilder);
    }
}