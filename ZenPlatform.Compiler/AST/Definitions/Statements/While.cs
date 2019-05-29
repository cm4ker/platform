using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class While : Statement
    {
        public Expression Condition;
        public InstructionsBodyNode InstructionsBody;

        public While(ILineInfo li, InstructionsBodyNode instructionsBody, Expression condition)
            : base(li)
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