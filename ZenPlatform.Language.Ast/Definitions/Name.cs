using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public partial class Name : Expression, ICanBeAssigned
    {
        public override TypeSyntax Type
        {
            get
            {
                var v = FirstParent<IScoped>().SymbolTable
                    .Find(this.Value, SymbolType.Variable, this.GetScope());
                return ((ITypedNode) v.SyntaxObject).Type;
            }
        }
    }


    public partial class AssignFieldExpression : ICanBeAssigned
    {
    }

    public interface ICanBeAssigned
    {
    }
}