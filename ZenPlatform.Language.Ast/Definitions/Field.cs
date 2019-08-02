using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    public partial class Field : IAstSymbol
    {
        public SymbolType SymbolType => SymbolType.Variable;
    }
}