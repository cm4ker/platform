using System.Collections.Generic;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Lowering;

internal class LambdaRewriter : GraphRewriter
{
    private readonly SourceMethodSymbolBase _method;
    private readonly PEModuleBuilder _moduleBuilder;
    private readonly CompilationState _compilationState;

    private readonly List<BoundVariable> _variablesAbove = new();
    private readonly NamedTypeSymbol _containingType;
    private readonly CoreTypes _ct;

    private LambdaRewriter(SourceMethodSymbolBase method, PEModuleBuilder moduleBuilder,
        CompilationState compilationState)
    {
        _method = method;
        _moduleBuilder = moduleBuilder;
        _compilationState = compilationState;
        _containingType = _method.ContainingType;
        _ct = moduleBuilder.Compilation.CoreTypes;
    }

    public override object VisitVariable(BoundVariable x)
    {
        _variablesAbove.Add(x);
        return base.VisitVariable(x);
    }

    public override object VisitFuncEx(BoundFuncEx x)
    {
        /*
            1. Create a new type-container for the lambda
               Remark: For container we need decide we need Context as a argument of method or we need context
               as a field of the type-container and pass it to the constructor of the type-container 
            2. Create required fields in type-container
            3. Create a constructor for the type-container
            4. Create a method for the lambda and transform it (replace all variables with fields)
            5. Return a new BoundCallEx represents the constructor of the type-container
        */
        var container =
            _moduleBuilder.SynthesizedManager.GetOrCreate<SynthesizedTypeSymbol>(_containingType, "LambdaContainer");
        
        var translatedSymbol = new TranslatedLambdaMethodSymbol(container, _method, x.LambdaSymbol);
        container.AddMember(translatedSymbol);
        _compilationState.RegisterMethodToEmit(translatedSymbol);

        return new BoundLiteral(1, _ct.Int32.Symbol);
    }

    public static void Transform(SourceMethodSymbolBase sourceMethodSymbolBase, PEModuleBuilder moduleBuilder,
        CompilationState state)
    {
        var rewriter = new LambdaRewriter(sourceMethodSymbolBase, moduleBuilder, state);
        var currentCFG = sourceMethodSymbolBase.ControlFlowGraph;
        var updatedCFG = (ControlFlowGraph)rewriter.VisitCFG(currentCFG);

        sourceMethodSymbolBase.ControlFlowGraph = updatedCFG;
    }
}

internal class TranslatedLambdaMethodSymbol : SourceMethodSymbolBase
{
    private readonly SourceMethodSymbolBase _lambda;

    public TranslatedLambdaMethodSymbol(Symbol containingSymbol, SourceMethodSymbolBase topLevelMethod,
        SourceMethodSymbolBase lambda) : base(containingSymbol)
    {
        _lambda = lambda;
    }

    public override Accessibility DeclaredAccessibility => Accessibility.Internal;

    public override bool IsStatic => true; // TODO: get answer from the context flow graph: IsHasCapturedVariables?

    internal override ParameterListSyntax SyntaxSignature => _lambda.SyntaxSignature;

    internal override TypeEx SyntaxReturnType => _lambda.SyntaxReturnType;

    internal override AquilaSyntaxNode Syntax => _lambda.Syntax;

    internal override IEnumerable<StmtSyntax> Statements => _lambda.Statements;

    public override ControlFlowGraph ControlFlowGraph => _lambda.ControlFlowGraph;

    public override TypeSymbol ReturnType => this.DeclaringCompilation.CoreTypes.Int32.Symbol;

    public override void GetDiagnostics(DiagnosticBag diagnostic)
    {
    }
}