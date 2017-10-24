namespace ZenPlatform.CSharpCodeBuilder.Syntax
{
    public class StringValueSyntax : ValueSyntax
    {
        private readonly string _value;

        public StringValueSyntax(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return $"\"{_value}\"";
        }
    }
}