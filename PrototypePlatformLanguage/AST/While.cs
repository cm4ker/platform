namespace PrototypePlatformLanguage.AST
{
    public class While : Statement
    {
        public Expression Condition = null;
        public MethodBody MethodBody = null;

        public While(MethodBody methodBody, Expression condition)
        {
            MethodBody = methodBody;
            Condition = condition;
        }
    }
}