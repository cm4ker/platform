using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Generation;

namespace ZenPlatform.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main2(args);
        }

        static void Main2(string[] args)
        {
            string test;
            var text = new StringReader(@"
module Test
{
    int Fibonachi(int n)
    {
        if(n == 0) return 0;
        if(n == 1) return 1;

        return Fibonachi(n-2) + Fibonachi(n-1);
    }

    double Average(int[] arr)
    {
        double result = 0.0;

        for(int i = 0; i < arr.Length; i++)
        {
            result = result + (double)(arr[i]);
        }  

        result = result / (double)(arr.Length);
        
        return result;
    }
}
");
            AntlrInputStream inputStream = new AntlrInputStream(text);
            ZSharpLexer lexer = new ZSharpLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            ZSharpParser parser = new ZSharpParser(commonTokenStream);

            parser.AddErrorListener(new Listener());
            ZLanguageVisitor visitor = new ZLanguageVisitor();
            var result = (CompilationUnit) visitor.VisitEntryPoint(parser.entryPoint());


            if (File.Exists("Debug.dll"))
                File.Delete("Debug.dll");
        }
    }


    static class Test
    {
        public static double M(int a, int b)
        {
            if (a > b)
            {
                a = a + 2;
            }
            else
            {
                a = a + 1;
            }

            ;


            return a;
        }

        static double Average(int[] arr)
        {
            double result = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                result = result + arr[i];
            }

            ;

            result = result / arr.Length;

            return result;
        }
    }
}