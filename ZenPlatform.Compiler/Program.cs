using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Cecil.Backend;
using ZenPlatform.Compiler.Generation;

namespace ZenPlatform.Compiler
{
    public class SimpleAR : BaseAssemblyResolver
    {
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            //Main2(args);
            TestAsm();
        }


        static void TestAsm()
        {
            var name = new AssemblyNameDefinition("Debug", new Version(1, 0));

            AssemblyDefinition ad =
                AssemblyDefinition.CreateAssembly(name, "Debug", new ModuleParameters
                {
                    Kind = ModuleKind.Dll,
                    AssemblyResolver = new CustomAssemblyResolver()
                });


            var sysAd = (new SimpleAR()).Resolve(new AssemblyNameReference("mscorlib", new Version(4, 0)));
            var arrType = sysAd.MainModule.ExportedTypes.First(x => x.Name == "Array" && x.Namespace == "System");
            var arrRef = new TypeReference("System", "Array", sysAd.MainModule, arrType.Scope);
            ad.MainModule.ImportReference(arrRef);

            if (File.Exists("Debug.dll"))
                File.Delete("Debug.dll");

            ad.Write("debug.dll");
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

            var name = new AssemblyNameDefinition("Debug", new Version(1, 0));

            AssemblyDefinition ad =
                AssemblyDefinition.CreateAssembly(name, "Debug", new ModuleParameters
                {
                    Kind = ModuleKind.Dll,
                    AssemblyResolver = new CustomAssemblyResolver()
                });

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