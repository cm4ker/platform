#nullable disable

using System;

namespace Aquila.CodeAnalysis.Syntax.InternalSyntax
{
    internal class DirectiveParser : SyntaxParser
    {
        private const int MAX_DIRECTIVE_IDENTIFIER_WIDTH = 128;

        private readonly DirectiveStack _context;

        internal DirectiveParser(Lexer lexer, DirectiveStack context)
            : base(lexer, LexerMode.Directive, null, null, false)
        {
            _context = context;
        }

        public AquilaSyntaxNode ParseDirective(
            bool isActive,
            bool endIsActive,
            bool isAfterFirstTokenInFile,
            bool isAfterNonWhitespaceOnLine)
        {
            throw new NotImplementedException();
        }
    }
}