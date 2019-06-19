using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class Do : Statement
    {
        public Expression Condition;
        public InstructionsBodyNode InstructionsBody;

        public Do(ILineInfo li, Expression condition, InstructionsBodyNode instructionsBody) : base(li)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Try : Statement
    {
        public Try(ILineInfo lineInfo, InstructionsBodyNode tryBlock, InstructionsBodyNode catchBlock,
            InstructionsBodyNode finallyBlock) : base(lineInfo)
        {
            TryBlock = tryBlock;
            CatchBlock = catchBlock;
            FinallyBlock = finallyBlock;
        }


        public InstructionsBodyNode TryBlock { get; set; }
        public InstructionsBodyNode CatchBlock { get; set; }
        public InstructionsBodyNode FinallyBlock { get; set; }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            visitor.Visit(TryBlock);
            visitor.Visit(CatchBlock);
            visitor.Visit(FinallyBlock);
        }
    }
}