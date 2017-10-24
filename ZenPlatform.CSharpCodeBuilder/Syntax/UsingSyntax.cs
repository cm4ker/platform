namespace ZenPlatform.CSharpCodeBuilder.Syntax
{
    public class UsingSyntax : Syntax
    {
        private readonly string _ns;

        public UsingSyntax(string ns)
        {
            _ns = ns;
        }

        public override string ToString()
        {
            return $"using {_ns};";
        }
    }
}