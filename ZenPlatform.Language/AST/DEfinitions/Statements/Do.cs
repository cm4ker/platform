namespace ZenPlatfrom.Language.AST.Definitions.Statements
{
    public class Do : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBody InstructionsBody = null;

        public Do(Infrastructure.Expression condition, InstructionsBody instructionsBody)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }
    }
}