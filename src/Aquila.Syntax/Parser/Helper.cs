using System.IO;
using Antlr4.Runtime;
using Aquila.Core.Network;
using Aquila.Language.Ast;

namespace Aquila.Compiler.Parser
{
    public static class ParseHelper
    {
        public static SyntaxNode ParseSyntax(string text)
        {
            var parser = Parse(text);
            ZLanguageVisitor zl = new ZLanguageVisitor();
            return zl.Visit(parser.entryPoint());
        }

        #region Helpers

        private static ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }

        private static ZSharpParser Parse(string text)
        {
            using TextReader tr = new StringReader(text);
            return Parse(tr);
        }

        public static ZSharpParser Parse(TextReader input)
        {
            return Parse(CreateInputStream(input));
        }

        private static ITokenStream CreateInputStream(TextReader reader)
        {
            Lexer lexer = new ZSharpLexer(new AntlrInputStream(reader));
            return new CommonTokenStream(lexer);
        }

        #endregion
    }
}