﻿using Aquila.CodeAnalysis;
using Aquila.Compiler.Test2;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Aquila.Syntax.Test;

public partial class SourceParserTest
{
   [Fact]
   public void ParseSimpleWebTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<h1></h1>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseSimpleWebWithContentTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<h1>HelloFromContent</h1>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseSimpleWebWithAttributeTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<h1 a=""value value value @value @value @(1+1)"" c=""test"">HelloFromContent</h1>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseWebMultiplyAttributeTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<t a=""@(1+1)"" c=""test @a""></t>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseWebAttributeWithJsonInsideTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<t a=""{test: 'abc' }""></t>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseSimpleWebWithContentExpressionTest()
   {
      var tp = new TreePrinter(_output);

      var graph = Parse(@"<h1>@(1 > hello_my_name(a,b)) sex</h1>");

      tp.Visit(graph.GetRoot());
   }
   
   
   [Fact]
   public void ParseSimpleWebEmptyTagTest()
   {
      var tp = new TreePrinter(_output);


      var graph = Parse(@"<h1 attribute />");
      tp.Visit(graph.GetRoot());
   }
   
      
   [Fact]
   public void ParseInnerHtmlText()
   {
      var tp = new TreePrinter(_output);


      var graph = Parse(@"<div><br /></div>");
      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseHtmlAndCode()
   {
      var tp = new TreePrinter(_output);
      var graph = Parse(@"<div><br /></div> @code { fn test_func() {} }");
      tp.Visit(graph.GetRoot());
   }

   private static SyntaxTree Parse(string code)
      => AquilaSyntaxTree.ParseText(code, new AquilaParseOptions(kind: SourceCodeKind.View));

}