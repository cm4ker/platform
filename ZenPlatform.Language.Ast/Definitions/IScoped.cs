using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public interface IScoped : IAstNode
    {
        SymbolTable SymbolTable { get; set; }
    }
}