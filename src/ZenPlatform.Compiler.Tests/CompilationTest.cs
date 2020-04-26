using Xunit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Tests
{
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


        [Fact]
        public void ExceptionTest()
        {
            var script = "string Main() { return \"Hello \" + \"world!\"; }";

            var result = (string) this.CompileAndRun(script);

            Assert.Equal("Hello world!", result);
        }

        [Fact]
        public void LookupTest()
        {
            var script =
                @"
namespace NS {
    type B
    {
        int PropB {get;set;}

        A PropA {get;set;}
    }
}
    type A
    {
        int Prop {get;set;}

        int Test(NS.B arg)
        {
           return arg.PropA.Prop;
        }
    }

";

            Compile(script);
        }


        [Fact]
        public void ifTest()
        {
            var script = "int Main() { if(10 > 1) return 1; else return 0;}";

            var result = (int) this.CompileAndRun(script);

            Assert.Equal(1, result);
        }


        [Fact]
        public void WhileTest()
        {
            var script = "int Main() { int i = 0; while(i < 10) i++; return i;}";

            var result = (int) this.CompileAndRun(script);

            Assert.Equal(10, result);
        }


        [Fact]
        public void ForTest()
        {
            var script = "int Main() {int a = 0; for(int i = 0; i < 10; i++){ a++; } return a;}";

            var result = (int) this.CompileAndRun(script);

            Assert.Equal(10, result);
        }

        [Fact]
        public void TryCatchTest()
        {
            var script =
                @"
int Main() 
{
 try{
    throw ""This is exception message"";
}
catch
{
return 1;
}
}";

            var result = (int) this.CompileAndRun(script);

            Assert.Equal(1, result);
        }
    }
}