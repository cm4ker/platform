using System.Collections.Generic;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
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

        public override void Accept(IVisitor visitor)
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

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(TryBlock);
            visitor.Visit(CatchBlock);
            visitor.Visit(FinallyBlock);
        }
    }
}