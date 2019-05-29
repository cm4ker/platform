using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions.Statements
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