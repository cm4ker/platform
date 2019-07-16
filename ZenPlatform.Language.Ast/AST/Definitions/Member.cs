using ZenPlatform.Compiler.Contracts.Symbols;


namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    /// <summary>
    /// Описывает член класса/структуры и т.д.
    /// </summary>
    public abstract class Member : AstNode
    {
        public Member(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}