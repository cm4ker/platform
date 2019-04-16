using System;
using System.IO;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatfrom.Language.AST;
using ZenPlatfrom.Language.AST.Definitions;
using ZenPlatfrom.Language.Generation;

namespace ZenPlatfrom.Language
{
    class Program
    {
        static void Main(string[] args)
        {
            Main2(args);
        }


        static void Main2(string[] args)
        {
            string test;
            var text = new StringReader(@"

module Test
{
    void Main()
    {
        int i = 123;
        string s = ""Hello \""world"";
    //    bool b = false;
        char c = 'V';
        double d = 123.23;
        
        int a = Mainly(i);
        Mainly(0);
    }

    int Mainly(int b)
    {
        int a = 2;
        return b + a;
    }
}

");

            AntlrInputStream inputStream = new AntlrInputStream(text);
            ZSharpLexer lexer = new ZSharpLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            ZSharpParser parser = new ZSharpParser(commonTokenStream);

            parser.AddErrorListener(new Listener());
            ZLanguageVisitor visitor = new ZLanguageVisitor();
            var result = (Module) visitor.VisitEntryPoint(parser.entryPoint());
            Generator g = new Generator(result);

            g.Compile("debug.dll");
        }
    }


    public class Listener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg,
            RecognitionException e)
        {
            Console.WriteLine($"Error at line {line} at char {charPositionInLine}: {msg}");

            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }

    static class Test
    {
        public static void Method()
        {
            string test;
        }
    }
}