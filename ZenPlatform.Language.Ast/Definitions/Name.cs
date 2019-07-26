using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public partial class Name : Expression
    {
    }

    public partial class FieldExpression : Expression
    {
        public FieldExpression(Expression exp, string fieldName) : this(null, exp, fieldName)
        {
        }
    }
}