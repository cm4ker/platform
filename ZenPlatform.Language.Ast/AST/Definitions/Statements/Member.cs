using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    /// <summary>
    /// Описывает член класса/структуры и т.д.
    /// </summary>
    public class Member : AstNode
    {
        public Member(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public override void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}