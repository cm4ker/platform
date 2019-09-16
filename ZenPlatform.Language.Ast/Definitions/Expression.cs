using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Выражение
    /// </summary>
    public partial class Expression : ITypedNode
    {
        public virtual TypeSyntax Type { get; set; }
    }
}