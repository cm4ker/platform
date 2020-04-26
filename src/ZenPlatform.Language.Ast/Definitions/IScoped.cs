using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public interface IScoped : IAstNode
    {
        SymbolTable SymbolTable { get; set; }
    }
}