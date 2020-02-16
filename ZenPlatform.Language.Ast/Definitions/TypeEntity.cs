using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public abstract partial class TypeEntity
    {
        public SymbolType SymbolType => SymbolType.Type;
    }
}