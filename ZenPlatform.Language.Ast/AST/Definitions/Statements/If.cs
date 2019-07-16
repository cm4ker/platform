using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class If : Statement
    {
        public Expression Condition;
        public BlockNode IfBlock;
        public BlockNode ElseBlock;

        public If(ILineInfo li, BlockNode elseBlock, BlockNode ifBlock,
            Expression condition)
            : base(li)
        {
            ElseBlock = elseBlock;
            IfBlock = ifBlock;
            Condition = condition;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitIf(this);
        }
    }
}