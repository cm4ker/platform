using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions
{
    public interface IScoped : IAstNode
    {
        SymbolTable SymbolTable { get; set; }
    }
}