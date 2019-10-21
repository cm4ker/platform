using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает модуль
    /// </summary>
    public partial class Module : TypeEntity
    {
        public SymbolScope SymbolScope { get; set; }
        
        public void AddFunction(Function function)
        {
            TypeBody.AddFunction(function);
        }
    }
}