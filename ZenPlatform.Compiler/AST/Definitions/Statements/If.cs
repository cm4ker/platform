namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    public class If : Statement
    {
        public Infrastructure.Expression Condition = null;
        public InstructionsBodyNode IfInstructionsBody = null;
        public InstructionsBodyNode ElseInstructionsBody = null;

        public If(InstructionsBodyNode elseInstructionsBody, InstructionsBodyNode ifInstructionsBody, Infrastructure.Expression condition)
        {
            ElseInstructionsBody = elseInstructionsBody;
            IfInstructionsBody = ifInstructionsBody;
            Condition = condition;
        }
    }
}