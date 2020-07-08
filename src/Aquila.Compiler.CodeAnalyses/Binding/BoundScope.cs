using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, Symbol>? _symbols;

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public BoundScope? Parent { get; }

        public bool TryDeclareVariable(LocalSymbol local)
            => TryDeclareSymbol(local);

        public bool TryDeclareFunction(MethodSymbol method)
            => TryDeclareSymbol(method);

        private bool TryDeclareSymbol<TSymbol>(TSymbol symbol)
            where TSymbol : Symbol
        {
            if (_symbols == null)
                _symbols = new Dictionary<string, Symbol>();
            else if (_symbols.ContainsKey(symbol.Name))
                return false;

            _symbols.Add(symbol.Name, symbol);
            return true;
        }

        public Symbol? TryLookupSymbol(string name)
        {
            if (_symbols != null && _symbols.TryGetValue(name, out var symbol))
                return symbol;

            return Parent?.TryLookupSymbol(name);
        }

        public ImmutableArray<LocalSymbol> GetDeclaredVariables()
            => GetDeclaredSymbols<LocalSymbol>();

        public ImmutableArray<MethodSymbol> GetDeclaredFunctions()
            => GetDeclaredSymbols<MethodSymbol>();

        private ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>()
            where TSymbol : Symbol
        {
            if (_symbols == null)
                return ImmutableArray<TSymbol>.Empty;

            return _symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
    }
}