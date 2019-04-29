namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class Do : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBodyNode InstructionsBody = null;

        public Do(Infrastructure.Expression condition, InstructionsBodyNode instructionsBody)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }
    }
}