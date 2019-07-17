using System.ComponentModel.Design.Serialization;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    /// <summary>
    /// Describes a statement.
    /// </summary>
    public abstract class Statement : AstNode
    {
        public Statement(ILineInfo lineInfo) : base(lineInfo)
        {
        }
    }
}