namespace PrototypePlatformLanguage.AST
{
    public class For : Statement
    {
        public Assignment Initializer = null;
        public Expression Condition = null;
        public Assignment Counter = null;
        public MethodBody MethodBody = null;

        public For(MethodBody methodBody, Assignment counter, Expression condition, Assignment initializer)
        {
            MethodBody = methodBody;
            Counter = counter;
            Condition = condition;
            Initializer = initializer;
        }
    }
}