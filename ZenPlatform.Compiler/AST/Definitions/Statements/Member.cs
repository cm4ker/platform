namespace ZenPlatform.Compiler.AST.Definitions.Statements
{
    /// <summary>
    /// Описывает член класса/структуры и т.д.
    /// </summary>
    public class Member : AstNode
    {
        public Member(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public Member(int line, int position) : base(line, position)
        {
        }
    }
}