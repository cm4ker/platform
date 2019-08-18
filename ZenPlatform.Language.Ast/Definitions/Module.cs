using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает модуль
    /// </summary>
    public partial class Module : TypeEntity
    {
        public SymbolScope SymbolScope { get; set; }
    }
}