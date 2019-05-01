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

 /*  void Main()
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

    double Div(int a, int b)
    {
        if(a > b)
        {
            a = a + 2;
        }
        else
        {
            a = a + 1;
           
        };

         return (double)a;
    }

    int Factor(int a)
    {
        int result = 1;
        for(int i = 1; i <= a; i++)
        {
            result = result * i;
        };

        a++;

        a = a + 1;
        return result;
    }

    int Counter(int a)
    {
        bool i = true;
        while ( i )
        {
           a++;

            if( a > 10) 
            {
                i = false;
            }
            else
            {
                a = Factor(a);
            };
        };
        
        return 0;
    }
    */

    int Average(int[] arr)
    {
        double result = 0;

        for(int i = 0; i < arr.Length; i++)
        {
            result = result + arr[i];
        };    

        result = result / arr.Length;
        
        return result;
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
            var result = (CompilationUnit) visitor.VisitEntryPoint(parser.entryPoint());

            Generator g = new Generator(result, ad);
            g.Emit();

            if (File.Exists("Debug.dll"))
                File.Delete("Debug.dll");

            ad.Write("debug.dll");
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


            return (double) a;
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