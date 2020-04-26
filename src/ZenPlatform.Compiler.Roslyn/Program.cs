using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    class Program
    {
        static void Main(string[] args)
        {
            RoslynAssemblyPlatform tp = new RoslynAssemblyPlatform();
            var asm = tp.AsmFactory.CreateAssembly(tp.TypeSystem, "test", new Version(1, 1, 1, 1));
            var sysb = new SystemTypeBindings(tp.TypeSystem);

            // var module = asm.DefineModule();
            var type = asm.DefineType("Namespace", "ClassName", TypeAttributes.Public, sysb.Object);

            var c = type.DefineConstructor(false);

            var intType = sysb.Int;
            var intToString = intType.Methods.First(x => x.Name == "ToString");
            var stringType = sysb.String;
            var doubleType = sysb.Double;
            var list = asm.TypeSystem.Resolve(typeof(List<>));
            var listOfStrings = list.MakeGenericType(stringType);


            var method = type.DefineMethod("Test", true, false, false);
            method.WithReturnType(intType);

            var param = method.DefineParameter("argus", intType, false, false);
            var param2 = method.DefineParameter("argus2", intType, false, false);

            var b = method.Body;

            var loc = b.DefineLocal(intType);
            var loc2 = b.DefineLocal(intType);
            var loc3 = b.DefineLocal(listOfStrings);

            var addMethod = listOfStrings.FindMethod("Add", sysb.String);

            b.NewObj(listOfStrings.Constructors.First())
                .StLoc(loc3)
                .LdLoc(loc3)
                .LdLit("This is for collection")
                .Call(addMethod)
                .Push(new BinaryExpression(new CastExpression(intType, new Literal(10.8)),
                    new Literal(10),
                    BKind.Plus))
                .StLoc(loc)
                .LdLit(10)
                .StArg(param)
                .Push(new Literal(10))
                .Inline(loc2)
                .LdLoc(loc2)
                .LdLit(5)
                .Cgt()
                .Inc(loc2);
            var b2 = b.Block();
            var someStringLoc = b2.DefineLocal(stringType);
            b2.Push(new Literal(10))
                .Call(intToString)
                .StLoc(someStringLoc)
                .EndBlock();
            b.For()
                .Nothing()
                .LdLit(11)
                .LdLit(12)
                .Call(method)
                .LdArg(param)
                .Ret();

            var sb = new StringBuilder();
            asm.Dump(new StringWriter(sb));

            EmitDemo.GenerateAssembly(sb.ToString(), Path.Combine(Directory.GetCurrentDirectory(), "mylib.bll"));

            asm.Dump(Console.Out);


            int i;
            for (i = 0; i < 10; i++)
            {
            }
        }
    }
}