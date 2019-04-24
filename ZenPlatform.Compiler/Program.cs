using System;
using System.IO;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Generation;

namespace ZenPlatform.Compiler
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
        if(a > b)
        {
            return (double)a / (double)b;
        }
        else
        {
            return 0.0;
        };
    }
}

");

            var name = new AssemblyNameDefinition("Debug.dll", new Version(1, 0));

            AssemblyDefinition ad = AssemblyDefinition.CreateAssembly(name, "Debug", ModuleKind.Dll);

            AntlrInputStream inputStream = new AntlrInputStream(text);
            ZSharpLexer lexer = new ZSharpLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            ZSharpParser parser = new ZSharpParser(commonTokenStream);

            parser.AddErrorListener(new Listener());
            ZLanguageVisitor visitor = new ZLanguageVisitor();
            var result = (Module) visitor.VisitEntryPoint(parser.entryPoint());

            Generator g = new Generator(result, ad);
            g.Emit();
            
            if(File.Exists("Debug.dll"))
                File.Delete("Debug.dll");
            
            ad.Write("debug.dll");
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