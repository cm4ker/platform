using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Statements
{
    public class While : Statement
    {
        public Expression Condition;
        public InstructionsBodyNode InstructionsBody;

        public While(ILineInfo li, InstructionsBodyNode instructionsBody, Expression condition)
            : base(li)
        {
            InstructionsBody = instructionsBody;
            Condition = condition;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}