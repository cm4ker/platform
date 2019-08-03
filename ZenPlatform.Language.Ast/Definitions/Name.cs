using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public partial class Name : Expression
    {
        public override TypeSyntax Type
        {
            get
            {
                var v = FirstParent<IScoped>().SymbolTable.Find(this.Value, SymbolType.Variable);
                return ((ITypedNode) v.SyntaxObject).Type;
            }
        }
    }
}