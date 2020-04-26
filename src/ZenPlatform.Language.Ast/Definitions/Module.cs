using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает модуль
    /// </summary>
    public partial class Module
    {
        public SymbolScopeBySecurity SymbolScope { get; set; }

        public void AddFunction(Function function)
        {
            TypeBody.Functions.Add(function);
        }
    }
}