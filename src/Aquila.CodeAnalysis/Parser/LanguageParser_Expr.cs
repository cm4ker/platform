using System.Diagnostics;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
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

        private ExprSyntax ParseCastOrParenExpression()
        {
            Debug.Assert(this.CurrentToken.Kind == SyntaxKind.OpenParenToken);

            var resetPoint = this.GetResetPoint();
            try
            {
                // We have a decision to make -- is this a cast, or is it a parenthesized
                // expression?  Because look-ahead is cheap with our token stream, we check
                // to see if this "looks like" a cast (without constructing any parse trees)
                // to help us make the decision.
                if (this.ScanCast())
                {
                    if (!IsCurrentTokenQueryKeywordInQuery())
                    {
                        // Looks like a cast, so parse it as one.
                        this.Reset(ref resetPoint);
                        var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                        var type = this.ParseType();
                        var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                        var expr = this.ParseSubExpression(Precedence.Cast);
                        return _syntaxFactory.CastEx(openParen, type, closeParen, expr);
                    }
                }

                // Doesn't look like a cast, so parse this as a parenthesized expression or tuple.
                {
                    this.Reset(ref resetPoint);
                    var openParen = this.EatToken(SyntaxKind.OpenParenToken);
                    var expression = this.ParseSubExpression(Precedence.Expression);

                    var closeParen = this.EatToken(SyntaxKind.CloseParenToken);
                    return _syntaxFactory.ParenthesizedEx(openParen, expression, closeParen);
                }
            }
            finally
            {
                this.Release(ref resetPoint);
            }
        }


        private bool IsPossibleFunctionExpression()
            => CurrentToken.Kind == SyntaxKind.FnKeyword
               && this.PeekToken(1).Kind == SyntaxKind.OpenParenToken;

            private FuncEx ParseAnonymousFunction()
        {
            var fnToken = EatToken(SyntaxKind.FnKeyword);

            var parameterList = ParseParenthesizedParameterList();
            var returnType = ParseType();
            var body = ParseMethodOrAccessorBodyBlock(new SyntaxList<AttributeListSyntax>(), false);
            return _syntaxFactory.FuncEx(fnToken, parameterList, returnType, body, null, null);
        }
    }
}