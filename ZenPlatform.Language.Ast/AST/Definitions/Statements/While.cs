using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class While : Statement
    {
        public Expression Condition;
        public BlockNode Block;

        public While(ILineInfo li, BlockNode block, Expression condition)
            : base(li)
        {
            Block = block;
            Condition = condition;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Match : Statement
    {
        public class MatchCondition
        {
            public Expression Condition { get; set; }

            public BlockNode Block { get; set; }
        }

        public Match(ILineInfo lineInfo) : base(lineInfo)
        {
            Cases = new List<MatchCondition>();
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
        }

        public Expression MatchExpression { get; set; }

        public List<MatchCondition> Cases { get; }

        public void AddCase(Expression expr, BlockNode block)
        {
            Cases.Add(new MatchCondition() {Condition = expr, Block = block});
        }
    }
}