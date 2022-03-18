using System;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis;
using Aquila.Core.Querying.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Aquila.Compiler.Test2
{
    public class TreePrinter : AquilaSyntaxWalker
    {
        private readonly ITestOutputHelper _output;

        public TreePrinter(ITestOutputHelper output)
        {
            _output = output;
        }

        static int Tabs = 0;

        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String('\t', Tabs);
            _output.WriteLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }
    }

    public class SourceParserTest
    {
        private readonly ITestOutputHelper _output;

        public SourceParserTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText("pub fn test() {}");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test2()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

pub fn main() int
{
    var a = 30;
       
    return match(a) { 
            | 10 => 1,
            | 20 => 2,
            | 30 => 3,
            | 40 => 4,
            |  _ => 100 };
}
");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void TestMatch()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
pub fn main() 
{
    print(match(30) { 
            | 10 => 1,
            | 20 => 2,
            | 30 => 3,
            | 40 => 4,
            |  _ => 100 });
}
");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void TestIf()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;
pub fn main() int 
{
    if(a > b)  
    {
        return 1;
    }
    else
    {
        return 2;
    }
}");

            tp.Visit(graph.GetRoot());
        }


        [Fact]
        public void Test4()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

pub fn main() int
{
    for(int i = 1; i < 10; i ++)
    {
        return 1;    
    } 
}");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test5()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

pub fn main()
{
    Call(1,2,3);
}");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test6()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

pub fn main()
{
    A.B();
}");
            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test8()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
pub fn main() int
{
    var c1 = list();
    var c2 = list<int>();
    var c3 = list<(int|string)>();

    c1.Add(""test"");
            c1.Add(""test2"");
   
            c2.Add(1);
            //c2.Add(""test""); //Error
   
            return c1.Count;
        }");

            tp.Visit(graph.GetRoot());
        }


        [Fact]
        public void Test9GetSyntaxByPosition()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
pub fn main() int
{
    var c1 = list();
    var c2 = list<int>();
    var c3 = list<(int|string)>();

    c1.Add(""test"");
            c1.Add(""test2"");
   
            c2.Add(1);
            //c2.Add(""test""); //Error
   
            return c1.Count;
        }");

            var token = graph.GetTouchingTokenAsync(9, CancellationToken.None).GetAwaiter().GetResult();

            Assert.True(token.Kind() == SyntaxKind.IdentifierToken);
            Assert.Equal("main", token.Text);
        }


        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TypeParseTest()
        {
            var graph = AquilaSyntaxTree.ParseText(
                @"type Test 
{
    int a;
    int b
    int c
    int d
}");
            Assert.True(graph.GetRoot().ChildNodes().First().Kind() == SyntaxKind.TypeDecl);
        }

        [Fact]
        public void TypeParseTest2()
        {
            var graph = AquilaSyntaxTree.ParseText(@"


type internal_type
{
    string message_hello;    
}

pub fn main() int
{
    return 0;    
}


");
        }

        [Fact]
        public void TypeCreateParse()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
pub fn main() some_type
{
    var calc_sec = sec{};
    var q = query{text = query_text, time_out = 30, sec = calc_sec};

    var a = some_type{ Prop = 1, Prop2 = 2, Prop3 = 6};
    var b = some_type{some, super, named, collections};

    return a;     
}
");

            tp.Visit(tree.GetRoot());
        }


        [Fact]
        public void FunctionParser()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
pub fn (i Invoice) set_shipping_date(date datetime)
{
         
}
");

            tp.Visit(tree.GetRoot());
        }

        [Fact]
        public void TypeParser()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
part type Invoice {}

pub fn (i Invoice) set_shipping_date(date datetime)
{
         
}
");
            tp.Visit(tree.GetRoot());
        }


        [Fact]
        public void TestUnaryNegExpr()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
fn main()
{ 
    println(-1);
    println(-(10+20)); 
}
");

            Assert.False(tree.GetDiagnostics().Any());
            tp.Visit(tree.GetRoot());
        }


        [Fact]
        public void ImportTest()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
import math;
import clr System.Console;
import clr System;
");
            tp.Visit(tree.GetRoot());
        }

        [Fact]
        public void ParseErrorTest()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
public static void Test()
{
}
");
            tp.Visit(tree.GetRoot());
        }
    }
}