using Aquila.Language.Ast.Extension;

namespace Aquila.Language.Ast.Symbols
{
    public abstract class TypeSymbol : NamespaceOrTypeSymbol
    {
        public virtual SpecialType SpecialType
        {
            get { return SpecialType.None; }
        }
    }
}