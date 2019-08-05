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
}