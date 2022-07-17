using Aquila.CodeAnalysis;
using Aquila.Compiler.Test2;
using Xunit;

namespace Aquila.Syntax.Test;

public partial class SourceParserTest
{
   [Fact]
   public void ParseSimpleWebTest()
   {
      var tp = new TreePrinter(_output);

      var graph = AquilaSyntaxTree.ParseText(@"<h1></h1>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseSimpleWebWithContentTest()
   {
      var tp = new TreePrinter(_output);

      var graph = AquilaSyntaxTree.ParseText(@"<h1>HelloFromContent</h1>");

      tp.Visit(graph.GetRoot());
   }
   
   [Fact]
   public void ParseSimpleWebWithContentExpressionTest()
   {
      var tp = new TreePrinter(_output);

      var graph = AquilaSyntaxTree.ParseText(@"<h1>@(1 > hello_my_name(a,b)) sex</h1>");

      tp.Visit(graph.GetRoot());
   }
}