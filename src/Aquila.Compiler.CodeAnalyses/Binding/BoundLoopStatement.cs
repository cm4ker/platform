namespace Aquila.Language.Ast.Binding
{
    internal abstract class BoundLoopStatement : BoundStatement
    {
        protected BoundLoopStatement(SyntaxNode syntax, BoundLabel breakLabel, BoundLabel continueLabel)
            : base(syntax)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }

        public BoundLabel BreakLabel { get; }
        public BoundLabel ContinueLabel { get; }
    }
}
