using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;

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
type HelloWorld
{
    int _someField;

    public int PublicMethod(object arg)
    {
        object a =  (2 + 2) * 3;
        object b = ""Hello epta""; 
        
        return (2 + 2) * 3 + 1;
    }
    
    [ClientCall]
    void PrivateMethod()
    {

    }

    public int Property {get; set;}
}

module TestModule
{
    int Test()
    {
        string Privet = ""test"";
    }
}
");

            CompilationBackend cb = new CompilationBackend();
            var b = cb.Compile(text);

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug.dll");
            var path = "Debug.dll";
            if (File.Exists(path))
                File.Delete(path);

            b.Write(path);
        }
    }
}