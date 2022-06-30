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
    private readonly LanguageParser _parser;
    private readonly LexerMode _mode;
    private Lexer _lexer;

    public WebParser(Lexer lexer, LanguageParser parser, ref LexerMode mode)
    {
        _parser = new LanguageParser(lexer, null, null, mode, CancellationToken.None);
        _mode = mode;
        _lexer = lexer;
    }

    internal SyntaxToken GetNext()
    {
        return _lexer.Lex(_mode);
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