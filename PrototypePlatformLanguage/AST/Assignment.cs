namespace PrototypePlatformLanguage.AST
{
    public class Assignment : Statement
    {
        public string Name;
        public Expression Value;
        public Expression Index;

        public Assignment(Expression value, Expression index, string name)
        {
            Value = value;
            Name = name;
            Index = index;
        }
    }
}