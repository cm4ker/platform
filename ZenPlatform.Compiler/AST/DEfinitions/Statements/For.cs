using ZenPlatform.Compiler.AST.Definitions.Functions;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class For : Statement
    {
        public Statement Initializer = null;
        public Infrastructure.Expression Condition = null;
        public Statement Counter = null;
        
        public InstructionsBody InstructionsBody = null;

        public For(InstructionsBody instructionsBody, Statement counter, Infrastructure.Expression condition, Statement initializer)
        {
            InstructionsBody = instructionsBody;
            Counter = counter;
            Condition = condition;
            Initializer = initializer;
        }
    }
}