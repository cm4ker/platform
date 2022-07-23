using System;
using Aquila.CodeAnalysis.Errors;
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
        while (true)
        {
            switch (CurrentToken.Kind)
            {
                case SyntaxKind.HtmlTextToken:
                    var text = ParseHtmlText();
                    nodes.Add(text);
                    break;
                case SyntaxKind.AtToken when IsCodeExpression():
                    var expression = ParseHtmlExpression();
                    nodes.Add(expression);
                    break;
                default:
                    return;
            }
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
                var equals = EatToken(SyntaxKind.EqualsToken);

                using (EnterMode(LexerMode.HtmlAttribute))
                {
                    var openQuote = EatToken(SyntaxKind.DoubleQuoteToken);
                    var nodes = _pool.Allocate<AquilaSyntaxNode>();
                    ParseAttributeNodes(nodes);
                    var closeQuote = EatToken(SyntaxKind.DoubleQuoteToken);

                    var attribute =
                        SyntaxFactory.HtmlAttribute(name, equals, openQuote, _pool.ToListAndFree(nodes), closeQuote);
                    attributes.Add(attribute);
                }
            }
            else
            {
                //HtmlAttribute simple identifier like
                attributes.Add(SyntaxFactory.HtmlAttribute(name, null, null, null, null));
            }
        }
    }

    private AquilaSyntaxNode ParseHtmlStartOrEmpty()
    {
        using (EnterMode(LexerMode.HtmlTag))
        {
            var lessThanToken = EatToken(SyntaxKind.LessThanToken);
            var htmlName = ParseHtmlName();
            var attributes = _pool.Allocate<HtmlAttributeSyntax>();
            ParseAttributes(attributes);

            if (CurrentToken.Kind == SyntaxKind.SlashGreaterThanToken)
            {
                var slashGreaterThanToken = EatToken(SyntaxKind.SlashGreaterThanToken);
                return SyntaxFactory.HtmlEmptyElement(lessThanToken, htmlName, _pool.ToListAndFree(attributes),
                    slashGreaterThanToken);
            }

            var greaterThanToken = EatToken(SyntaxKind.GreaterThanToken);

            return SyntaxFactory.HtmlElementStartTag(lessThanToken, htmlName, _pool.ToListAndFree(attributes),
                greaterThanToken);
        }
    }

    private ContextMode EnterMode(LexerMode mode, bool @override = false)
    {
        var context = new ContextMode(Mode, this);
        if (@override)
            Mode = mode;
        else
            Mode |= mode;

        return context;
    }

    private class ContextMode : IDisposable
    {
        private readonly LanguageParser _parser;
        public LexerMode Mode { get; }

        public ContextMode(LexerMode mode, LanguageParser parser)
        {
            _parser = parser;
            Mode = mode;
        }

        public void Exit()
        {
            _parser.Mode = Mode;
        }

        public void Dispose()
        {
            Exit();
        }
    }

    private HtmlElementEndTagSyntax ParseHtmlEnd()
    {
        using (EnterMode(LexerMode.HtmlTag))
        {
            var lessThanSlshToken = EatToken(SyntaxKind.LessThanSlashToken);
            var htmlName = ParseHtmlName();
            var greatThanToken = EatToken(SyntaxKind.GreaterThanToken);
            return SyntaxFactory.HtmlElementEndTag(lessThanSlshToken, htmlName, greatThanToken);
        }
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

        using (EnterMode(LexerMode.Syntax, true))
        {
            var stmt = ParseStatementCore(null, false);
            return SyntaxFactory.HtmlStatement(atToken, stmt);
        }
    }

    private bool IsPrimaryHtmlExpression()
    {
        return PeekToken(1).Kind != SyntaxKind.OpenParenToken;
    }

    HtmlExpressionSyntax ParseHtmlExpression()
    {
        var atToken = EatToken(SyntaxKind.AtToken);
        ExprSyntax expr;

        using (EnterMode(LexerMode.Syntax, true))
        {
            if (IsPrimaryHtmlExpression())
                expr = ParseTerm(Precedence.Expression);
            else
            {
                var openParen = EatToken(SyntaxKind.OpenParenToken);
                expr = ParseExpression();
                var closeParen = EatToken(SyntaxKind.CloseParenToken);
                expr = _syntaxFactory.ParenthesizedEx(openParen, expr, closeParen);
            }
        }

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
                    var node = ParseHtmlTagNode();
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

    private HtmlNodeSyntax ParseHtmlTagNode()
    {
        var startOrEmpty = ParseHtmlStartOrEmpty();

        switch (startOrEmpty.Kind)
        {
            case SyntaxKind.HtmlEmptyElement:
                return (HtmlEmptyElementSyntax)startOrEmpty;
            case SyntaxKind.HtmlElementStartTag:
                var nodes = _pool.Allocate<HtmlNodeSyntax>();
                ParseHtmlContent(nodes);
                var end = ParseHtmlEnd();

                return SyntaxFactory.HtmlElement((HtmlElementStartTagSyntax)startOrEmpty, _pool.ToListAndFree(nodes),
                    end);
            default:
                AddError(startOrEmpty, ErrorCode.Unknown);
                return null;
        }
    }
}