using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions
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