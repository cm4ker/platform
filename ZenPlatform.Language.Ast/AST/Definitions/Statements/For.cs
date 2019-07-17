using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class For : Statement
    {
        private const int INI_SLOT = 0;
        private const int CONDITION_SLOT = 1;
        private const int COUNTER_SLOT = 2;
        private const int BLOCK_SLOT = 3;

        public Expression Initializer { get; }
        public Expression Condition { get; }
        public Expression Counter { get; }
        public BlockNode Block { get; }

        public For(ILineInfo li, BlockNode block, Expression counter, Expression condition,
            Expression initializer) : base(li)
        {
            Block = Children.SetSlot(block, BLOCK_SLOT);
            Counter = Children.SetSlot(counter, COUNTER_SLOT);
            Condition = Children.SetSlot(condition, CONDITION_SLOT);
            Initializer = Children.SetSlot(initializer, INI_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitFor(this);
        }
    }
}