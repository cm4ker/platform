namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class While : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBodyNode InstructionsBody = null;

        public While(InstructionsBodyNode instructionsBody, Infrastructure.Expression condition)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }
    }
}