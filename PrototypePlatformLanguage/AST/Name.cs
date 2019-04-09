namespace PrototypePlatformLanguage.AST
{
    public class Name : Expression
    {
        public string Value;

        public Name(string value)
        {
            Value = value;
        }
    }
}