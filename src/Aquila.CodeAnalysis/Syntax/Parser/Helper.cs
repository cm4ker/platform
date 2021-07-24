using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Syntax.Parser;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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
        public static SourceUnit ParseUnit(string text, string fileName)
        {
            var parser = Parse(text);
            ZLanguageVisitor zl = new ZLanguageVisitor(fileName);

            var ast = zl.Visit(parser.entryPoint()) as SourceUnit;
            return ast;
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