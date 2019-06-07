using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public class Symbol
    {
        public string Name;
        public SymbolType Type;
        public IAstSymbol SyntaxObject;
        public object CodeObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Symbel name</param>
        /// <param name="type">Symbol kind type</param>
        /// <param name="syntaxObject"><see cref="AstNode"/> object</param>
        /// <param name="codeObject">The connected object from the codegeneration backend</param>
        public Symbol(string name, SymbolType type, IAstSymbol syntaxObject, object codeObject)
        {
            Name = name;
            Type = type;
            SyntaxObject = syntaxObject;
            CodeObject = codeObject;
        }
    }
}