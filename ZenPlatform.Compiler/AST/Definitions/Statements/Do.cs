using ZenPlatform.Compiler.AST.Infrastructure;
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
}