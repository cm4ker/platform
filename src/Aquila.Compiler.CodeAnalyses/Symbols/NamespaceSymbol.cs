namespace Aquila.Language.Ast.Symbols
{
    public abstract class NamespaceSymbol : NamespaceOrTypeSymbol
    {
        public override SymbolKind Kind => SymbolKind.Namespace;
    }
}