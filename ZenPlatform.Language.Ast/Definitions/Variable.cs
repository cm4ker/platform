using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Describes a variable.
    /// </summary>
    public partial class Variable : ITypedNode
    {
        public SymbolType SymbolType => SymbolType.Variable;
        public SymbolScopeBySecurity SymbolScope { get; set; }


        public Variable(ILineInfo li, Expression value, string name) : this(li, value, name, value.Type)
        {
        }
    }
}