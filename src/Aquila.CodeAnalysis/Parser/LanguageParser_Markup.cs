using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax;

internal partial class LanguageParser
{
    private HtmlNameSyntax ParseHtmlName()
    {
        var tagName = EatToken(SyntaxKind.IdentifierToken);
        return SyntaxFactory.HtmlName(tagName);
    }

    private HtmlTextSyntax ParseHtmlText()
    {
        var htmlText = EatToken(SyntaxKind.HtmlTextToken);
        return SyntaxFactory.HtmlText(htmlText);
    }

    private void ParseAttributeNodes(SyntaxListBuilder<AquilaSyntaxNode> nodes)
    {
        switch (CurrentToken.Kind)
        {
            case SyntaxKind.HtmlText:
                var text = ParseHtmlText();
                nodes.Add(text);
                break;
            case SyntaxKind.AtToken when IsCodeExpression():
                var rp = GetResetPoint();
                Mode = LexerMode.Syntax;
                var expression = ParseExpression();
                Mode = rp.BaseResetPoint.Mode;
                nodes.Add(expression);
                break;
        }
    }

    private bool IsCodeExpression()
    {
        return CurrentToken.Kind == SyntaxKind.AtToken
               && PeekToken(1).Kind != SyntaxKind.AtToken;
    }

    private bool IsEmptyHtmlElement() => this.CurrentToken.Kind == SyntaxKind.SlashGreaterThanToken;

    private void ParseAttributes(SyntaxListBuilder<HtmlAttributeSyntax> attributes)
    {
        while (true)
        {
            if (CurrentToken.Kind != SyntaxKind.IdentifierToken)
                break;


            var name = ParseHtmlName();

            if (CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                //this is valued attribute
                var equasl = EatToken(SyntaxKind.EqualsToken);
                var openQuote = EatToken(SyntaxKind.DoubleQuoteToken);
                var nodes = _pool.Allocate<AquilaSyntaxNode>();
                ParseAttributeNodes(nodes);
                var closeQuote = EatToken(SyntaxKind.DoubleQuoteToken);

                var attribute =
                    SyntaxFactory.HtmlAttribute(name, equasl, openQuote, _pool.ToListAndFree(nodes), closeQuote);
                attributes.Add(attribute);
            }
            else
            {
                //HtmlAttribute simple identifier like
                SyntaxFactory.HtmlAttribute(name, null, null, null, null);
            }
        }
    }

    private HtmlElementStartTagSyntax ParseHtmlStart()
    {
        var lessThanToken = EatToken(SyntaxKind.LessThanToken);
        var htmlName = ParseHtmlName();
        var attributes = _pool.Allocate<HtmlAttributeSyntax>();
        ParseAttributes(attributes);
        var greaterThanToken = EatToken(SyntaxKind.GreaterThanToken);

        return SyntaxFactory.HtmlElementStartTag(lessThanToken, htmlName, _pool.ToListAndFree(attributes),
            greaterThanToken);
    }

    private HtmlElementEndTagSyntax ParseHtmlEnd()
    {
        var lessThanSlshToken = EatToken(SyntaxKind.LessThanSlashToken);
        var htmlName = ParseHtmlName();
        var greatThanToken = EatToken(SyntaxKind.GreaterThanToken);

        return SyntaxFactory.HtmlElementEndTag(lessThanSlshToken, htmlName, greatThanToken);
    }

    private bool IsHtmlStatement()
    {
        return CurrentToken.Kind == SyntaxKind.AtToken
               && (PeekToken(1).Kind == SyntaxKind.ForKeyword
                   || PeekToken(1).Kind == SyntaxKind.WhileKeyword
                   || PeekToken(1).Kind == SyntaxKind.OpenBraceToken
                   || PeekToken(1).Kind == SyntaxKind.IfKeyword);
    }

    HtmlStatementSyntax ParseHtmlStatement()
    {
        var atToken = EatToken(SyntaxKind.AtToken);
        var rp = GetResetPoint();
        Mode = LexerMode.Syntax;
        var stmt = ParseStatementCore(null, false);
        Mode = rp.BaseResetPoint.Mode;
        return SyntaxFactory.HtmlStatement(atToken, stmt);
    }

    private bool IsPrimaryHtmlExpression()
    {
        return PeekToken(1).Kind != SyntaxKind.OpenParenToken;
    }

    HtmlExpressionSyntax ParseHtmlExpression()
    {
        var atToken = EatToken(SyntaxKind.AtToken);
        var mode = Mode;
        Mode = LexerMode.Syntax;
        ExprSyntax expr;
        if (IsPrimaryHtmlExpression())
            expr = ParseTerm(Precedence.Expression);
        else
        {
            var openParen = EatToken(SyntaxKind.OpenParenToken);
            expr = ParseExpression();
            var closeParen = EatToken(SyntaxKind.CloseParenToken);
            expr = _syntaxFactory.ParenthesizedEx(openParen, expr, closeParen);
        }
        
        Mode = mode;
        return SyntaxFactory.HtmlExpression(atToken, expr);
    }

    private HtmlNodeSyntax ParseHtmlInterrupt()
    {
        if (IsHtmlStatement())
        {
            return ParseHtmlStatement();
        }
        else
        {
            //TODO: ParseHtmlCode();
            return ParseHtmlExpression();
        }
    }

    private void ParseHtmlContent(SyntaxListBuilder<HtmlNodeSyntax> contentNodes)
    {
        while (true)
            switch (CurrentToken.Kind)
            {
                case SyntaxKind.LessThanToken:
                    var node = ParseHtmlNode();
                    contentNodes.Add(node);
                    break;
                case SyntaxKind.AtToken:
                    var code = ParseHtmlInterrupt();
                    contentNodes.Add(code);
                    break;
                case SyntaxKind.HtmlTextToken:
                    var text = ParseHtmlText();
                    contentNodes.Add(text);
                    break;
                default:
                    return;
            }
    }

    private HtmlNodeSyntax ParseHtmlNode()
    {
        if (IsEmptyHtmlElement())
        {
            var lessThanToken = EatToken(SyntaxKind.LessThanToken);
            var htmlName = ParseHtmlName();
            var attributes = _pool.Allocate<HtmlAttributeSyntax>();
            ParseAttributes(attributes);
            var slashGreaterThanToken = EatToken(SyntaxKind.SlashGreaterThanToken);

            return SyntaxFactory.HtmlEmptyElement(lessThanToken, htmlName, _pool.ToListAndFree(attributes),
                slashGreaterThanToken);
        }
        else
        {
            var start = ParseHtmlStart();
            var nodes = _pool.Allocate<HtmlNodeSyntax>();
            ParseHtmlContent(nodes);
            var end = ParseHtmlEnd();

            return SyntaxFactory.HtmlElement(start, _pool.ToListAndFree(nodes), end);
        }
    }
}