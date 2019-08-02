using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.AST.Definitions.Symbols
{
    public interface ISymbol
    {
        string Name { get; set; }
        SymbolType Type { get; set; }
        IAstSymbol SyntaxObject { get; set; }
        object CodeObject { get; set; }
        T GetSyntaxObject<T>() where T : IAstNode;
    }

    public class Symbol : ISymbol
    {
        public string Name { get; set; }
        public SymbolType Type { get; set; }
        public IAstSymbol SyntaxObject { get; set; }
        public object CodeObject { get; set; }

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


        public T GetSyntaxObject<T>() where T : IAstNode
        {
            return (T) SyntaxObject;
        }
    }
}