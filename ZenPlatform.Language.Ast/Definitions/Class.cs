using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
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
            TypeBody.AddFunction(function);
        }
    }
}