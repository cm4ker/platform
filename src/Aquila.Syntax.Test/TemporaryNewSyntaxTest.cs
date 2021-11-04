using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace Aquila.Compiler.Test2
{
    public class TreePrinter : CSharpSyntaxWalker
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

            var graph = CSharpSyntaxTree.ParseText("public void test() {}");

            tp.Visit(graph.GetRoot());
        }

        [Fact]
        public void Test2()
        {
            var tp = new TreePrinter(_output);

            var graph = CSharpSyntaxTree.ParseText(
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
    }
}