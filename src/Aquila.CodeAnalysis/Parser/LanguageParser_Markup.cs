using System;
using Aquila.CodeAnalysis.Errors;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax;

internal partial class LanguageParser
{
    /*
     this part of LanguageParser response for parsing
     html constructions while describing view
     like a 
     <html>
        Hello this is view, @user_name
     </html>
     */

    #region Mode controller

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

    #endregion

    private HtmlNodeSyntax ParseHtmlNode()
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

    private HtmlNameSyntax ParseHtmlName()
    {
        var tagName = EatToken(SyntaxKind.IdentifierToken);
        return SyntaxFactory.HtmlName(tagName);
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
                case SyntaxKind.AtToken when IsHtmlCodeExpression():
                    var expression = ParseHtmlExpression();
                    nodes.Add(expression);
                    break;
                default:
                    return;
            }
        }
    }

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

    private bool IsHtmlCodeExpression()
    {
        return CurrentToken.Kind == SyntaxKind.AtToken
               && PeekToken(1).Kind != SyntaxKind.AtToken;
    }

    private bool IsHtmlCodeStatement()
    {
        return CurrentToken.Kind == SyntaxKind.AtToken
               && (PeekToken(1).Kind == SyntaxKind.ForKeyword
                   || PeekToken(1).Kind == SyntaxKind.WhileKeyword
                   || PeekToken(1).Kind == SyntaxKind.OpenBraceToken
                   || PeekToken(1).Kind == SyntaxKind.IfKeyword);
    }

    private bool IsHtmlCodeDecl()
    {
        return CurrentToken.Kind == SyntaxKind.AtToken
               && PeekToken(1).Kind == SyntaxKind.HtmlCodeKeyword;
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

    HtmlCodeSyntax ParseHtmlCode()
    {
        var atToken = EatToken(SyntaxKind.AtToken);
        var codeKeyword = EatToken(SyntaxKind.HtmlCodeKeyword);
        MemberDecl memberDecl;

        using (EnterMode(LexerMode.Syntax, true))
        {
            var openBrace = EatToken(SyntaxKind.OpenBraceToken);

            var members = _pool.Allocate<MemberDecl>();
            while ((memberDecl = ParseMemberDeclaration(SyntaxKind.CompilationUnit)) != null)
            {
                members.Add(memberDecl);
            }

            var closeBrace = EatToken(SyntaxKind.CloseBraceToken);

            return SyntaxFactory.HtmlCode(atToken, codeKeyword, openBrace, members.ToList(), closeBrace);
        }
    }

    private bool IsPrimaryHtmlExpression()
    {
        return PeekToken(1).Kind != SyntaxKind.OpenParenToken;
    }

    private HtmlNodeSyntax ParseHtmlInterrupt()
    {
        if (IsHtmlCodeStatement())
        {
            return ParseHtmlStatement();
        }

        return ParseHtmlExpression();
    }

    private HtmlDecl ParseHtmlDecl()
    {
        var nodes = _pool.Allocate<HtmlNodeSyntax>();
        HtmlCodeSyntax htmlCode = null;

        ParseHtmlContent(nodes);

        if (IsHtmlCodeDecl())
            htmlCode = ParseHtmlCode();
        
        return _syntaxFactory.HtmlDecl(_syntaxFactory.HtmlMarkupDecl(_pool.ToListAndFree(nodes)), htmlCode);
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
                case var _ when IsHtmlCodeDecl():
                    return;
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
}