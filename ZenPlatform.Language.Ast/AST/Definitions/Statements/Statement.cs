using System.ComponentModel.Design.Serialization;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    /// <summary>
    /// Describes a statement.
    /// </summary>
    public abstract class Statement : SyntaxNode
    {
        public Statement(ILineInfo lineInfo) : base(lineInfo)
        {
        }
    }
}