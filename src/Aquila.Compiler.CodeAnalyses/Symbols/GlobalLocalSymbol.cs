using Aquila.Language.Ast.Binding;

namespace Aquila.Language.Ast.Symbols
{
    public sealed class GlobalLocalSymbol : LocalSymbol
    {
        internal GlobalLocalSymbol(string name, bool isReadOnly, NamedTypeSymbol namedType, BoundConstant? constant)
            : base(name, isReadOnly, namedType, constant)
        {
        }

        public override SymbolKind Kind => SymbolKind.Global;
    }
}