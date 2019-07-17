using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class Do : Statement
    {
        private const int CONDITION_SLOT = 0;
        private const int BLOCK_SLOT = 1;

        private readonly Expression _condition;
        private readonly BlockNode _block;

        public Expression Condition => _condition;

        public BlockNode Block => _block;

        public Do(ILineInfo li, Expression condition, BlockNode block) : base(li)
        {
            _block = Children.SetSlot(block, BLOCK_SLOT);
            _condition = Children.SetSlot(condition, CONDITION_SLOT);
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitDoStatement(this);
        }
    }

    public class Try : Statement
    {
        public Try(ILineInfo lineInfo, BlockNode tryBlock, BlockNode catchBlock,
            BlockNode finallyBlock) : base(lineInfo)
        {
            TryBlock = tryBlock;
            CatchBlock = catchBlock;
            FinallyBlock = finallyBlock;
        }

        public BlockNode TryBlock { get; set; }
        public BlockNode CatchBlock { get; set; }
        public BlockNode FinallyBlock { get; set; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}