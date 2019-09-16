using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Tests
{
    public class AstWalkerTest
    {
        public class SimpleAstWalker : AstVisitorBase<SyntaxNode>
        {
            public SimpleAstWalker()
            {
                FoundedLiterals = new List<string>();
            }

            public List<string> FoundedLiterals { get; }

            public override SyntaxNode VisitLiteral(Literal obj)
            {
                FoundedLiterals.Add(obj.Value);
                return base.VisitLiteral(obj);
            }
        }


        public void SimpleLiteralTest()
        {
            var test =
                @"module Test 
{
    void SomeVoidName()
    {
        string var1 = ""First literal"";
    }
}";
        }
    }

    public class CompilationTest : TestBaseCLR
    {
        [Fact]
        public void SimpleExpression()
        {
            var script = "int Main() { return 2 + 2 * 2; }";

            var result = (int) this.CompileAndRun(script);

            Assert.Equal(6, result);
        }

        [Fact]
        public void SimpleStringConcatinationTestExpression()
        {
            var script = "string Main() { return \"Hello \" + \"world!\"; }";

            var result = (string) this.CompileAndRun(script);

            Assert.Equal("Hello world!", result);
        }
    }
}