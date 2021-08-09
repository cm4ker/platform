using Aquila.CodeAnalysis.Syntax;
using Aquila.CodeAnalysis.Syntax.Parser;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Declarations;
using Aquila.Syntax.Parser;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;

namespace Aquila.Compiler.Test2
{
    public class SourceProviderTest
    {
        private ZLanguageVisitor _v;

        private readonly ITestOutputHelper _output;

        public SourceProviderTest(ITestOutputHelper output)
        {
            _v = new ZLanguageVisitor("" ,"unknown");
            _output = output;
        }

        [Fact]
        public void ExtendsTest()
        {
            var parser =
                Parse(
                    "component A {extend Test{ void SomeMethod1() {}  } extend Test2{ void Twice(){} } extend Test{ void SomeMethod2() {} }}");

            var res1 = _v.Visit(parser.entryPoint());
            parser.Reset();
            var res2 = _v.Visit(parser.entryPoint());

            var unit = Assert.IsAssignableFrom<SourceUnit>(res1);
            var unit2 = Assert.IsAssignableFrom<SourceUnit>(res2);

            AquilaSyntaxTree st = AquilaSyntaxTree.Create(unit, "test.cs");

            SyntaxTree st2 = AquilaSyntaxTree.Create(unit2, "test2.cs");

            var sp = new MergedSourceUnit(new[] {unit, unit2});
            var components = sp.GetComponents();

            foreach (var component in components)
            {
                _output.WriteLine($"For Component {component.Identifier.Text} we have extends:");

                foreach (var extend in component.GetExtends())
                {
                    _output.WriteLine($"\tFor extend {extend.Identifier.Text} we have next methods:");
                    foreach (var method in extend.Methods)
                    {
                        _output.WriteLine(
                            $"\t\tmt = {method.Identifier.Text}; parent = {method.Parent.Parent.Span} from {method.SyntaxTree.FilePath}");
                    }
                }
            }
        }

        #region Helpers

        private ZSharpParser Parse(string text)
        {
            return ParserHelper.Parse(text);
        }

        #endregion
    }
}