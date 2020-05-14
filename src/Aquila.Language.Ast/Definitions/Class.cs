using System.Runtime.CompilerServices;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public partial class Class
    {
        public bool ImplementsReference { get; set; }
        public SymbolScopeBySecurity SymbolScope { get; set; }


        public void AddFunction(Function function)
        {
            TypeBody.Functions.Add(function);
        }
    }
}