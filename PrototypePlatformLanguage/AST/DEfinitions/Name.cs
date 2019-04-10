namespace PrototypePlatformLanguage.AST.Definitions
{
    public class Name : Infrastructure.Expression
    {
        public string Value;

        public Name(string value)
        {
            Value = value;
        }
    }
}