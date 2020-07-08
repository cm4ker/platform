using System.Collections.Immutable;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(BoundProgram? previous,
                            ImmutableArray<Diagnostic> diagnostics,
                            MethodSymbol? mainFunction,
                            MethodSymbol? scriptFunction,
                            ImmutableDictionary<MethodSymbol, BoundBlockStatement> functions)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
        }

        public BoundProgram? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public MethodSymbol? MainFunction { get; }
        public MethodSymbol? ScriptFunction { get; }
        public ImmutableDictionary<MethodSymbol, BoundBlockStatement> Functions { get; }
    }
}
