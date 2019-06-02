using System;
using System.IO;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.Language.AST;
using ZenPlatform.Language.AST.Definitions;
using ZenPlatform.Language.Generation;

namespace ZenPlatform.Language
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

/*    void Main()
    {
        double testCast = (double)1;
    }

    int Multiply(int a, int b)
    {
        return a * b;
    }

    int Add(int a, int b)
    {
        return b + a;
    }

    int Sub(int a, int b)
    {
        return a - b;
    }
*/
    double Div(int a, int b)
    {
        return (double)a / (double)b;
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

            if (File.Exists("BetaName.dll"))
                File.Delete("BetaName.dll");
            g.Compile("BetaName.dll");
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