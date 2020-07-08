using Aquila.Language.Ast.Binding;

namespace Aquila.Language.Ast.Symbols
{
    public class LocalLocalSymbol : LocalSymbol
    {
        internal LocalLocalSymbol(string name, bool isReadOnly, NamedTypeSymbol namedType, BoundConstant? constant)
            : base(name, isReadOnly, namedType, constant)
        {
        }
    }
}