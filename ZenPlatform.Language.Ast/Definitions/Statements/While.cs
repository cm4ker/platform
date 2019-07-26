using System;
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class While : Statement
    {
        public Expression Condition;
        public Block Block;

        public While(ILineInfo li, Block block, Expression condition)
            : base(li)
        {
            Block = block;
            Condition = condition;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Match : Statement
    {
        public class MatchCondition
        {
            public Expression Condition { get; set; }

            public Block Block { get; set; }
        }

        public Match(ILineInfo lineInfo) : base(lineInfo)
        {
            Cases = new List<MatchCondition>();
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }

        public Expression MatchExpression { get; set; }

        public List<MatchCondition> Cases { get; }

        public void AddCase(Expression expr, Block block)
        {
            Cases.Add(new MatchCondition() {Condition = expr, Block = block});
        }
    }
}