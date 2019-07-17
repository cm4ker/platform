using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class If : Statement
    {
        private const int CONDITION_SLOT = 0;
        private const int IF_THEN_SLOT = 1;
        private const int ELSE_SLOT = 2;

        private Expression _condition;
        private readonly BlockNode _ifBlock;
        private readonly BlockNode _elseBlock;

        public Expression Condition => _condition;

        public BlockNode IfBlock => _ifBlock;

        public BlockNode ElseBlock => _elseBlock;

        public If(ILineInfo li, BlockNode elseBlock, BlockNode ifBlock,
            Expression condition)
            : base(li)
        {
            _elseBlock = Children.SetSlot(elseBlock, ELSE_SLOT);
            _ifBlock = Children.SetSlot(ifBlock, IF_THEN_SLOT);
            _condition = Children.SetSlot(condition, CONDITION_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIf(this);
        }
    }
}