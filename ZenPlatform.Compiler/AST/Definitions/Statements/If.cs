using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class If : Statement
    {
        public Expression Condition;
        public InstructionsBodyNode IfInstructionsBody;
        public InstructionsBodyNode ElseInstructionsBody;

        public If(ILineInfo li, InstructionsBodyNode elseInstructionsBody, InstructionsBodyNode ifInstructionsBody,
            Expression condition)
            : base(li)
        {
            ElseInstructionsBody = elseInstructionsBody;
            IfInstructionsBody = ifInstructionsBody;
            Condition = condition;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Condition);
            visitor.Visit(IfInstructionsBody);
            visitor.Visit(ElseInstructionsBody);
        }
    }
}