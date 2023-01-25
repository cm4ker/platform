using System.Collections.Generic;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols;

internal sealed class SourceLambdaSymbol : SourceMethodSymbolBase
{
    private readonly FuncEx _functionExpr;

    public SourceLambdaSymbol(NamedTypeSymbol type, FuncEx functionExpr) : base(type)
    {
        _functionExpr = functionExpr;
    }

    public override Accessibility DeclaredAccessibility => Accessibility.Internal;
    
    public override bool IsStatic => false;

    internal override ParameterListSyntax SyntaxSignature => _functionExpr.ParameterList;
    
    internal override TypeEx SyntaxReturnType => _functionExpr.ReturnType;
    
    internal override AquilaSyntaxNode Syntax => _functionExpr;
    
    internal override IEnumerable<StmtSyntax> Statements => _functionExpr.Body?.Statements;

    protected override Binder GetMethodBinder()
    {
        return DeclaringCompilation.GetBinder(_functionExpr);
    }

    public override void GetDiagnostics(DiagnosticBag diagnostic)
    {

    }
}