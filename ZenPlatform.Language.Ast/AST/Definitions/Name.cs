using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Именованоое поле (переменная). В последствии раскручивания дерева эта переменная запишется в таблицу символов
    /// </summary>
    public class Name : Expression
    {
        public string Value;

        public Name(ILineInfo li, string value) : base(li)
        {
            Value = value;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitName(this);
        }
    }

    public class FieldExpression : Expression
    {
        private const int EXPRESSION_SLOT = 0;


        private Expression _expression;

        public Expression Expression => _expression ?? Children.GetSlot(out _expression, EXPRESSION_SLOT);

        public string Name { get; }

        public FieldExpression(Expression expression, string name) : base(null)
        {
            _expression = Children.SetSlot(expression, EXPRESSION_SLOT);
            Name = name;
            Line = expression.Line;
            Position = expression.Position;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFieldExpression(this);
        }
    }
}