using System;
using System.IO;
using Antlr4.Runtime;

namespace Aquila.Syntax.Parser
{
    public class MyErrListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            throw new ArgumentException("Invalid Expression: {0}", msg, e);
        }

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg,
            RecognitionException e)
        {
            throw new ArgumentException("Invalid Expression: {0}", msg, e);
        }
    }

    public static class ParserHelper
    {
        public static LangElement ParseSyntax(string text)
        {
            var parser = Parse(text);
            ZLanguageVisitor zl = new ZLanguageVisitor();
            return zl.Visit(parser.entryPoint());
        }

        #region Helpers

        private static ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);
            parser.ErrorHandler = new DefaultErrorStrategy();
            parser.AddErrorListener(new Listener());

            return parser;
        }

        public static ZSharpParser Parse(string text)
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

            var ts = new CommonTokenStream(lexer);

            return ts;
        }

        #endregion
    }
}