namespace ZenPlatform.Compiler.Contracts.Symbols
{
    public class Symbol
    {
        public string Name;

        public IAstSymbol SyntaxObject;
        public object CodeObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Symbel name</param>
        /// <param name="type">Symbol kind type</param>
        /// <param name="syntaxObject"><see cref="AstNode"/> object</param>
        /// <param name="codeObject">The connected object from the codegeneration backend</param>
        public Symbol(string name, IAstSymbol syntaxObject, object codeObject)
        {
            Name = name;
            SyntaxObject = syntaxObject;
            CodeObject = codeObject;
        }
    }
}