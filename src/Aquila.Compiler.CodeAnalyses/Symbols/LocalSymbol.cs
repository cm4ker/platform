using Aquila.Language.Ast.Binding;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class LocalSymbol : Symbol
    {
        internal LocalSymbol(string name, bool isReadOnly, NamedTypeSymbol namedType, BoundConstant? constant)
        {
            IsReadOnly = isReadOnly;
            NamedType = namedType;
            Constant = isReadOnly ? constant : null;
        }

        public bool IsReadOnly { get; }

        public override SymbolKind Kind => SymbolKind.Local;

        public NamedTypeSymbol NamedType { get; }
        internal BoundConstant? Constant { get; }
    }
}