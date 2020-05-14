using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Выражение
    /// </summary>
    public partial class Expression : ITypedNode
    {
        public virtual TypeSyntax Type { get; set; }
    }

    public partial class LookupExpression
    {
        public override TypeSyntax Type
        {
            get { return Lookup.Type; }
        }
    }
}