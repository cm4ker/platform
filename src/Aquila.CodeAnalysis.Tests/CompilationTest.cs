using Xunit;

namespace Aquila.Compiler.Tests
{
    public class CompilationTest : CompilerTestBase
    {
        [Fact]
        public void SimpleExpression()
        {
            var script = "static int Main() { return 1 + 2 * 3 + 1; }";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(8, result);
        }

        [Fact]
        public void SimpleStringConcatinationTestExpression()
        {
            var script = "static string Main() { return \"Hello \" + \"world!\"; }";

            var result = (string)this.CompileAndRun(script);

            Assert.Equal("Hello world!", result);
        }


        [Fact]
        public void SimpleAssingExpressionString()
        {
            var script = "static int Main() { string a = \"HELLO \" + \"WORLD\"; return a.Length; }";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal("Hello world".Length, result);
        }

        [Fact]
        public void SimpleAssingExpression()
        {
            var script = "static int Main() { int a = 9 + 1; return a; }";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(10, result);
        }


        [Fact]
        public void BinderImportingTypeTest()
        {
            var script =
                @"
import Entity;

static int Main() 
{ 
    Invoice i = 0; 
    return 1; 
}";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(1, result);
        }


        [Fact]
        public void SimpleCallTestExpression()
        {
            var script = @"
static void Main() 
{ 
    Callable(); 
} 

static int Callable() 
{ 
    return  1 + 1; 
}";

            this.CompileAndRun(script);
        }


        [Fact]
        public void SimpleCallWithArgExpression()
        {
            var script =
                @"
static int Main() 
{ 
    return Callable(2); 
} 
static int Callable(int i) 
{
    return  i * 2; 
}
";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(4, result);
        }

        [Fact]
        public void SimpleCallWithTwoArgsExpression()
        {
            var script =
                @"
static int Main() 
{ 
    return Mul(10, 12); 
} 
static int Mul(int x, int y) 
{
    return  x * y; 
}
";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(120, result);
        }


        [Fact]
        public void ExceptionTest()
        {
            var script = "string Main() { return \"Hello \" + \"world!\"; }";

            var result = (string)this.CompileAndRun(script);

            Assert.Equal("Hello world!", result);
        }

        [Fact]
        public void ifTest()
        {
            var script = @"
static bool Test() 
{
    return true;
} 

static int Main() 
{
    if(Test()) 
        return 1; 
    else 
        return 0;
}";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(1, result);
        }


        [Fact]
        public void WhileTest()
        {
            var script = @"
static int Main() 
{ 
    int i = 0;
    int a = 1;
    
    while(i < 10)
    {
        i = i + 2;
        a = a * i + a;
    } 
    return a;

}";

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(10395, result);
        }

        [Fact]
        public void InjectionTest()
        {
            var script = @"
public static int Main() 
{ 
    int i = 0;
    int a = 1;
    
    while(i < 10)
    {
        i = i + 2;
        a = a * i + a;
    } 
 
    return a;

}

component Entity
{
    extend InvoiceObject
    {
        public void A()
        {
            // this is call from the public method
            Save();
        }

        private void B()
        {
            // this is call from the private method
            Save();
        }
    }
}

";

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(10395, result);
        }

        [Fact]
        public void ForTest()
        {
            var script = @"
static int Main() 
{
    int a = 0; 

    for(int i = 0; i < 10; i++)
    {
        a++; 
    }
 
    return a;
}";

            var result = (int)this.CompileAndRun(script);

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

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(1, result);
        }


        [Fact]
        public void UsingTest()
        {
            var script =
                @"
type Test
{
    using System;

    int Main() 
    {
        var g = new NullReferenceException();
        
        return 1;
    }
}";

            this.Compile(script);
        }


        [Fact]
        public void QueryParsingTest()
        {
            var script =
                @"
                 
public static int Main() 
{
    string q = q"" FROM Entity.Invoice SELECT Id "" ;

    return 0;
}

";

            var result = (int)this.CompileAndRun(script);
        }
    }
}