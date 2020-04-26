using System;
using System.IO;
using Antlr4.Runtime;
using ZenPlatform.Compiler.Preprocessor;
using ZenPlatform.Language.Ast;

namespace ZenPlatform.Compiler.Tests
{
    public static class TransformZSharpHelper
    {
        public static SyntaxNode Parse(this string text, Func<ZSharpParser, SyntaxNode> a)
        {
            return a(Parse(text));
        }

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

        private static ITokenStream CreateInputStream(Stream input)
        {
            return PreProcessor.Do(new AntlrInputStream(input));
        }

        private static ITokenStream CreateInputStream(TextReader reader)
        {
            return PreProcessor.Do(new AntlrInputStream(reader));
        }
    }
}