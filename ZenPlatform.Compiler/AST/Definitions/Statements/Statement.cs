namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    /// <summary>
    /// Describes a statement.
    /// </summary>
    public class Statement : AstNode
    {
        public Statement(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public Statement(int line, int position) : base(line, position)
        {
        }
    }
}