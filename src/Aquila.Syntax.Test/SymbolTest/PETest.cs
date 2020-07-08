// using System.Collections.Generic;
// using System.Linq;
// using Aquila.Syntax.Ast;
// using Aquila.Syntax.Declarations;
// using Aquila.Syntax.Parser;
// using Aquila.Syntax.Semantic;
// using Aquila.Syntax.Symbols;
// using Mono.Cecil;
// using Xunit;
//
// namespace Aquila.Compiler.Test2.SymbolTest
// {
//     public static class TmpHelper
//     {
//         public static BaseAssemblyResolver Resolver = new DefaultAssemblyResolver();
//
//         public static CoreLibAssemblySymbol Corelib =
//             new CoreLibAssemblySymbol(Resolver.Resolve(AssemblyNameReference.Parse("mscorlib")));
//     }
//
//     public class CompilationTest
//     {
//         [Fact]
//         public void TestLiteralOp()
//         {
//             var sf = new SemanticFactory(TmpHelper.Corelib);
//             var tree = ParserHelper.ParseSyntax(@"void test() {int a = 10 + 10 + ""Literal"";} ");
//             var ops = sf.Visit(tree.Syntax);
//         }
//     }
//
//     public class PETest
//     {
//         [Fact]
//         public void SimpleLoadAsm()
//         {
//             BaseAssemblyResolver r = new DefaultAssemblyResolver();
//             var asm = r.Resolve(AssemblyNameReference.Parse("mscorlib"));
//
//             PEAssemblySymbol sym = new PEAssemblySymbol(asm);
//             var ns = sym.GetNamespaces().ToList();
//
//             var systemNs = ns[1];
//             var nested = systemNs.GetNestedNamespaces().ToList();
//
//             var int32 = sym.FindType("System.Object");
//         }
//
//         [Fact]
//         public void TestCorelibAsm()
//         {
//             BaseAssemblyResolver r = new DefaultAssemblyResolver();
//             var asm = r.Resolve(AssemblyNameReference.Parse("mscorlib"));
//             CoreLibAssemblySymbol s = new CoreLibAssemblySymbol(asm);
//             var i32 = s.Int32;
//             Assert.NotNull(i32);
//
//             var methods = i32.GetMethods().ToList();
//
//             Assert.True(methods.Any());
//         }
//
//         [Fact]
//         public void TryToWriteSimpleAsm()
//         {
//             var s = new SrcAssemblySymbol(new MergedSourceUnit(new List<SourceUnit>()));
//             s.SetName("TestAsm")
//                 .SetVersion("1.2.3.4");
//
//             var t = s.TypeManager.SynthesizeType();
//             t.SetName("A.TestType");
//             t.SetPublic(true);
//
//             s.Dump("TestAsm.dll");
//         }
//     }
// }