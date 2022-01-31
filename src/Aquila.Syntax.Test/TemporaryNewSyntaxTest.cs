using System;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis;
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

    public class TemporaryNewSyntaxTest
    {
        private readonly ITestOutputHelper _output;

        public TemporaryNewSyntaxTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText("public void test() {}");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test2()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

public static int Main() 
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
        public void Test3()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
import Entity;

public static int Main() 
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

public static int Main() 
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

public static int Main() 
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

public static int Main() 
{
    A.B();
}");
            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test7()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
extend Invoice
{
    public void Sum()
    {
        
    }

    public void Sum()
    {
        
    }
}");
            tp.Visit(graph.GetRoot());
        }


        [Fact]
        public void Test8()
        {
            var tp = new TreePrinter(_output);

            var graph = AquilaSyntaxTree.ParseText(
                @"
public static int Main() 
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
public static int Main() 
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

            var token = graph.GetTouchingTokenAsync(20, CancellationToken.None).GetAwaiter().GetResult();

            Assert.True(token.Kind() == SyntaxKind.IdentifierToken);
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
public static int Main()
{
    return 0;    
}

type internal_type
{
    string message_hello;    
}

");
        }

        [Fact]
        public void TypeCreateParse()
        {
            var tp = new TreePrinter(_output);
            var tree = AquilaSyntaxTree.ParseText(@"
public static some_type Main()
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
    }
}