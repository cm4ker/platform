using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Web;

internal class WebParser
{
    private LexerMode _mode;
    private Lexer _lexer;

    public WebParser(Lexer lexer, ref LexerMode mode)
    {
        _mode = mode;
        _lexer = lexer;
    }

    internal Action ParseLanguage { get; set; }

    internal SyntaxToken GetNext()
    {
        var token = _lexer.Lex(ref _mode);

        if (token.Kind == SyntaxKind.MarkupInterruptToken)
        {
            ParseLanguage?.Invoke();
        }

        return token;
    }

    public void ParseTag()
    {
        // StartTag
        // Attributes
        // Possible empty tag

        // Content
        // Tag
        // AquilaCode
        // Comment ( different comments style! )

        // EndTag
    }
}