namespace ZenPlatfrom.Language.AST.Definitions.Symbols
{
    public class Symbol
    {
        public string Name;
        public SymbolType Type = SymbolType.None;
        public object SyntaxObject;
        public object CodeObject;

        public Symbol(string name, SymbolType type, object syntaxObject, object codeObject)
        {
            Name = name;
            Type = type;
            SyntaxObject = syntaxObject;
            CodeObject = codeObject;
        }
    }
}