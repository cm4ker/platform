using System.Collections.Immutable;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope? previous,
            ImmutableArray<Diagnostic> diagnostics,
            MethodSymbol? mainFunction,
            MethodSymbol? scriptFunction,
            ImmutableArray<MethodSymbol> functions,
            ImmutableArray<LocalSymbol> variables)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
            Variables = variables;
        }

        public BoundGlobalScope? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public MethodSymbol? MainFunction { get; }
        public MethodSymbol? ScriptFunction { get; }
        public ImmutableArray<MethodSymbol> Functions { get; }
        public ImmutableArray<LocalSymbol> Variables { get; }
        public ImmutableArray<BoundStatement> Statements { get; }
    }
}