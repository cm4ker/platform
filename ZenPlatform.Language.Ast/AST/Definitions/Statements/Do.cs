using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class Do : Statement
    {
        public Expression Condition;
        public BlockNode Block;

        public Do(ILineInfo li, Expression condition, BlockNode block) : base(li)
        {
            Block = block;
            Condition = condition;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
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