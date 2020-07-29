using Aquila.Compiler.Dnlib;
using Aquila.Language.Ast;
using Xunit;

namespace Aquila.Compiler.Test2
{
    public class TreeTest
    {
        [Fact]
        public void TestSyntaxTree()
        {
            SyntaxTree tree = SyntaxTree.Parse("void A(){ a = 10; }");

            Assert.NotNull(tree.Root);
            Assert.Equal(1, tree.Root.Methods.Count);
        }


        [Fact]
        public void CompiltaionTest()
        {
            SyntaxTree tree = SyntaxTree.Parse("void A(){ a = 10; }"); // related to programm

            var c = Compilation.Create(new DnlibAssemblyPlatform(), tree);
            var boundedProgram = c.GetProgram();

            Assert.Empty(boundedProgram.Diagnostics);
        }
    }
}