using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public partial class Class
    {
        public bool ImplementsReference { get; set; }
        public SymbolScopeBySecurity SymbolScope { get; set; }


        public void AddFunction(Method method)
        {
            //TypeBody.Functions.Add(method);
        }
    }
}