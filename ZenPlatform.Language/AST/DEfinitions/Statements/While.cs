namespace ZenPlatform.Language.AST.Definitions.Statements
{
    public class While : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBody InstructionsBody = null;

        public While(InstructionsBody instructionsBody, Infrastructure.Expression condition)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }
    }
}