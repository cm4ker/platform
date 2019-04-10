namespace PrototypePlatformLanguage.AST.Definitions.Statements
{
    public class If : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBody IfInstructionsBody = null;
        public InstructionsBody ElseInstructionsBody = null;

        public If(InstructionsBody elseInstructionsBody, InstructionsBody ifInstructionsBody, Infrastructure.Expression condition)
        {
            ElseInstructionsBody = elseInstructionsBody;
            IfInstructionsBody = ifInstructionsBody;
            Condition = condition;
        }
    }
}