using System.Collections.Generic;
using System.Threading;
using Aquila.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.CodeAnalysis.Parser;

internal class ViewParser : SyntaxParser
{
    public ViewParser(Lexer lexer, LexerMode mode)
        : base(lexer, LexerMode.SyntaxView, null, null, true)
    {
        
    }
}