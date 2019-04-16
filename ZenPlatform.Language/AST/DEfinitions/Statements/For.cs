using ZenPlatfrom.Language.AST.Definitions.Functions;

namespace ZenPlatfrom.Language.AST.Definitions.Statements
{
    public class For : Statement
    {
        public Assignment Initializer = null;
        public Infrastructure.Expression Condition = null;
        public Assignment Counter = null;
        public InstructionsBody InstructionsBody = null;

        public For(InstructionsBody instructionsBody, Assignment counter, Infrastructure.Expression condition, Assignment initializer)
        {
            InstructionsBody = instructionsBody;
            Counter = counter;
            Condition = condition;
            Initializer = initializer;
        }
    }
}