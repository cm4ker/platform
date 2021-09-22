using System;
using Aquila.Core;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Sessions;
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
    Invoice|int i = 0; 
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
        public void a()
        {
            // this is call from the public method
            save();
        }

        private void b()
        {
            // this is call from the private method
            save();
        }

        private void before_save()
        {   
            var a = 0;

            Name = ""Кириллица"";
        }
    }
}

";

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(10395, result);
        }

        [Fact]
        public void VarDeclTest()
        {
            var script = @"
public static int Main() 
{ 
    return 1;
}

component Entity
{
    extend InvoiceObject
    {
        private void before_save()
        {   
            var a = 0;
        }
    }
}

";

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(1, result);
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

        [Fact]
        public void UnionTypeTest()
        {
            var script =
                @"
import Entity;

public static int Main() 
{
    int|string|Invoice a = 10;
    string|int b = ""test"";

    return 0;
}";

/*
public static string Main() 
{
    int|string a = 10;
    string|int b = "test";

    var l1 = list<int>();
    var l2 = dict<int, string>();
    var l3 = arr<int>();
    
    var l = list();
       
    l.add(1);
    l.add(ref);
    
    l1.add(1);       -- ok
    l1.add("test");  -- error!
    
    return match a with
        | int x => "here is string value"
        | string y => "here is y value";
}                 
*/


            var result = (int)this.CompileAndRun(script);
        }


        [Fact]
        public void ExtensionMethodTest()
        {
            var script =
                @"
public static int Main() 
{
    var a = 1;
    var b = a;
    
    var q = query();
    var d = get_date();
    q.text = ""from a select b"";
    q.set_param(""value"", ""value"");
    q.exec();

    return d.Day;
}";

            var d = DateTime.Now.Day;
            var result = (int)this.CompileAndRun(script);
            var da = DateTime.Now.Day;

            int expected;

            if (d != da)
                expected = da;
            else
                expected = d;

            Assert.Equal(expected, result);
        }


        [Fact]
        public void TestCollections()
        {
            var script =
                @"
public static int Main() 
{
    var c1 = list();
    var c2 = list<int>();
    var c3 = list<int|string>();

    c1.Add(""test"");
    c1.Add(""test2"");
   
    c2.Add(1);
    c2.Add(""test""); //Error
   
    return c1.Count;
    return 0;
}";

            var result = (int)this.CompileAndRun(script);

            Assert.Equal(2, result);
        }


        [Fact]
        public void QueryTestDBReaderAccess()
        {
            var script =
                @"
public static int Main() 
{
    var q = query();
    q.text = ""from Entity.Invoice select Id"";
    q.set_param(""param_name"", 1);

    var r = q.exec();
    if(r.read())
    {
        var id = r[""Id11""];
        var name = r[""Name""];
        var summ = r[""Sum""];

        return 1;
    }

    return 0;
}";

            var d = DateTime.Now.Day;

            var result = (int)this.CompileAndRun(script);
            var da = DateTime.Now.Day;
        }


        [Fact]
        public void MatchTest()
        {
            var script =
                @"
public static int Main() 
{
    var a = 10;
       
    return match(a)
            | 10 => 1
            | 20 => 2
            | 30 => 3
            | 40 => 4
            |  _ => 100;

}";

            var d = DateTime.Now.Day;

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(1, result);
        }

        [Fact]
        public void Match2Test()
        {
            var script =
                @"
public static int Main() 
{
    var a = false;
       
    return match(a)
            | true => 0
            | false => 1;
}";

            var d = DateTime.Now.Day;

            var result = (int)this.CompileAndRun(script);
            Assert.Equal(1, result);
        }
    }
}