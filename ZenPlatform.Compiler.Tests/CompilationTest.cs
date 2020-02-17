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

type B
{
    int PropB {get;set;}

    A PropA {get;set;}
}

type A
{
    int Prop {get;set;}

    int Test(B arg)
    {
       return arg.PropA.Prop;
    }
}
";

            Compile(script);
        }

        [Fact]
        public void BindingTest()
        {
            ClassTable ct = new ClassTable();
            ct.FillStandard(Ap.TypeSystem.GetSystemBindings());

            var res = ct.FindType("int");
            Assert.Equal("System.Int32", res.FullName);

            res = ct.FindType("Platform", "Int64");

            Assert.Equal("System.Int64", res.FullName);

            res = ct.FindType("Platform.Int64");

            Assert.Equal("System.Int64", res.FullName);

            res = ct.FindType("Platform.SomeNamespace.Int64");

            Assert.Equal("System.Int64", res.FullName);

            res = ct.FindType("Platform.SomeNamespace", "Int64");
            Assert.Equal("System.Int64", res.FullName);
        }
    }
}