using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class For : Statement
    {
        public Statement Initializer;
        public Expression Condition;
        public Statement Counter;

        public BlockNode Block;

        public For(ILineInfo li, BlockNode block, Statement counter, Expression condition,
            Statement initializer) : base(li)
        {
            Block = block;
            Counter = counter;
            Condition = condition;
            Initializer = initializer;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(Counter);
            visitor.Visit(Condition);
            visitor.Visit(Initializer);
            visitor.Visit(Block);
        }
    }
}