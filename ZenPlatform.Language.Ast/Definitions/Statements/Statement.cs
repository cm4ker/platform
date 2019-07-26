using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    /// <summary>
    /// Describes a statement.
    /// </summary>
    public abstract  partial class Statement : SyntaxNode
    {
        public Statement(ILineInfo lineInfo) : base(lineInfo)
        {
        }
    }
}