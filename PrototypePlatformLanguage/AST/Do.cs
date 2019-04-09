namespace PrototypePlatformLanguage.AST
{
    public class Do : Statement
    {
        public Expression Condition = null;
        public MethodBody MethodBody = null;

        public Do(Expression condition, MethodBody methodBody)
        {
            MethodBody = methodBody;
            Condition = condition;
        }
    }
}