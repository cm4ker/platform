using Xunit;

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

type NS.B
{
    int PropB {get;set;}
}

type NS.A
{
    int Prop {get;set;}

    int Test(NS.B arg)
    {
       return arg.PropB;
    }
}
";

            Compile(script);
        }
    }
}