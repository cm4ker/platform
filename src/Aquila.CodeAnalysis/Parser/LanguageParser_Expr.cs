using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

    partial class LanguageParser
    {
        private MatchEx ParseMatch()
        {
            Debug.Assert(CurrentToken.Kind == SyntaxKind.MatchKeyword);

            var matchKeyword = EatToken(SyntaxKind.MatchKeyword);
            var openParen = EatToken(SyntaxKind.OpenParenToken);
            var expr = ParseExpressionCore();
            var closeParen = EatToken(SyntaxKind.CloseParenToken);
            var openBrace = EatToken(SyntaxKind.OpenBraceToken);

            //parse arms here
            var arms = ParseArms();

            var closeBrace = EatToken(SyntaxKind.CloseBraceToken);

            return _syntaxFactory.MatchEx(matchKeyword, openParen, expr, closeParen, openBrace, arms,
                closeBrace);
        }

        private SeparatedSyntaxList<MatchArm> ParseArms()
        {
            var arms = _pool.AllocateSeparated<MatchArm>();

            while (CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
                var barToken = EatToken(SyntaxKind.BarToken);
                var patternExpr = ParseExpressionCore();
                var greatThenEqualsToken = EatToken(SyntaxKind.EqualsGreaterThanToken);
                var resultExpression = ParseExpressionCore();

                var arm = _syntaxFactory.MatchArm(barToken, patternExpr, greatThenEqualsToken, resultExpression);

                // If we're not making progress, abort
                if (arm.Width == 0 && this.CurrentToken.Kind != SyntaxKind.CommaToken)
                    break;

                arms.Add(arm);
                if (this.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    var commaToken = this.CurrentToken.Kind == SyntaxKind.SemicolonToken
                        ? this.EatTokenAsKind(SyntaxKind.CommaToken)
                        : this.EatToken(SyntaxKind.CommaToken);
                    arms.AddSeparator(commaToken);
                }
            }

            return arms;
        }
    }
}