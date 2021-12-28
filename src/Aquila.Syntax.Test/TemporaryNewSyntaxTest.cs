﻿using System;
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

        [Fact]
        public void Test3()
        {
            var tp = new TreePrinter(_output);

            var graph = CSharpSyntaxTree.ParseText(
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

            var graph = CSharpSyntaxTree.ParseText(
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

            var graph = CSharpSyntaxTree.ParseText(
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

            var graph = CSharpSyntaxTree.ParseText(
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

            var graph = CSharpSyntaxTree.ParseText(
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

            var graph = CSharpSyntaxTree.ParseText(
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
    }
}