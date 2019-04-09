namespace PrototypePlatformLanguage.AST
{
    public class If : Statement
    {
        public Expression Condition = null;
        public MethodBody IfMethodBody = null;
        public MethodBody ElseMethodBody = null;

        public If(MethodBody elseMethodBody, MethodBody ifMethodBody, Expression condition)
        {
            ElseMethodBody = elseMethodBody;
            IfMethodBody = ifMethodBody;
            Condition = condition;
        }
    }
}