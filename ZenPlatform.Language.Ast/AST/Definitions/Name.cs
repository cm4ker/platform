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

        public override void Accept(IVisitor visitor)
        {
            //Nothing to do
        }
    }

    public class FieldExpression : Expression
    {
        public Expression Expression { get; }
        public string Name { get; }

        public FieldExpression(Expression expression, string name) : base(null)
        {
            Expression = expression;
            Name = name;
            Line = expression.Line;
            Position = expression.Position;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Expression);
        }
    }
}