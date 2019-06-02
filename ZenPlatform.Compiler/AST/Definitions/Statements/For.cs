using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class For : Statement
    {
        public Statement Initializer;
        public Expression Condition;
        public Statement Counter;

        public InstructionsBodyNode InstructionsBody;

        public For(ILineInfo li, InstructionsBodyNode instructionsBody, Statement counter, Expression condition,
            Statement initializer) : base(li)
        {
            InstructionsBody = instructionsBody;
            Counter = counter;
            Condition = condition;
            Initializer = initializer;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(Counter);
            visitor.Visit(Condition);
            visitor.Visit(Initializer);
            visitor.Visit(InstructionsBody);
        }
    }
}