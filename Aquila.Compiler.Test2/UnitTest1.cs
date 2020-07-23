using System;
using System.IO;
using System.Xml.XPath;
using Antlr4.Runtime;
using Aquila.Compiler.Parser;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Infrastructure;
using Xunit;

namespace Aquila.Compiler.Test2
{
    public class UnitTest1
    {
        private ZLanguageVisitor _v;

        public UnitTest1()
        {
            _v = new ZLanguageVisitor();
        }

        [Fact]
        public void LiteralExpressionTest()
        {
            var parser = Parse("10");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<ExpressionSyntax>(result);
        }

        [Fact]
        public void BinaryExpressionTest()
        {
            var parser = Parse("10 + 1");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<BinaryExpressionSyntax>(result);
        }

        [Fact]
        public void UnaryExpressionTest()
        {
            var parser = Parse("!a");
            var result = _v.VisitExpression(parser.expression());
            var expr = Assert.IsAssignableFrom<UnaryExpressionSyntax>(result);
            Assert.Equal(UnaryOperatorType.Not, expr.OperaotrType);
        }

        [Fact]
        public void DeclareVariableTest()
        {
            var parser = Parse("int a = 10");
            var result = _v.VisitLocal_variable_declaration(parser.local_variable_declaration());
            var expr = Assert.IsAssignableFrom<VariableDeclarationSyntax>(result);
        }

        [Fact]
        public void AssignTest()
        {
            var parser = Parse("a = 10");
            var result = _v.VisitAssignment(parser.assignment());
            var expr = Assert.IsAssignableFrom<AssignmentSyntax>(result);
        }

        [Fact]
        public void BlockTest()
        {
            var parser = Parse("{int a = 10; a = 20; {int c = 300;}}");
            var result = _v.VisitBlock(parser.block());
            var expr = Assert.IsAssignableFrom<BlockSyntax>(result);
            Assert.Equal(3, expr.Statements.Count);
            Assert.IsAssignableFrom<BlockSyntax>(expr.Statements[2]);
        }

        [Fact]
        public void IfTest()
        {
            var parser = Parse("if(true) a = 10 else a = 20");
            var result = _v.VisitEmbedded_statement(parser.embedded_statement());
            var expr = Assert.IsAssignableFrom<If>(result);
        }

        [Fact]
        public void MethodDeclarationTest()
        {
            var parser = Parse("public void A() {}");
            var result = _v.Visit(parser.method_declaration());
            var expr = Assert.IsAssignableFrom<MethodDeclarationSyntax>(result);
        }

        [Fact]
        public void CompilationUnitTest()
        {
            var parser = Parse("public void A() {} public void b() {}");
            var result = _v.Visit(parser.entryPoint());
            var expr = Assert.IsAssignableFrom<CompilationUnitSyntax>(result);
            Assert.Equal(2, expr.Methods.Count);
        }

        #region Helpers

        private ZSharpParser Parse(ITokenStream tokenStream)
        {
            ZSharpParser parser = new ZSharpParser(tokenStream);

            parser.AddErrorListener(new Listener());

            return parser;
        }

        private ZSharpParser Parse(string text)
        {
            using TextReader tr = new StringReader(text);
            return Parse(tr);
        }

        public ZSharpParser Parse(TextReader input)
        {
            return Parse(CreateInputStream(input));
        }

        private ITokenStream CreateInputStream(TextReader reader)
        {
            Lexer lexer = new ZSharpLexer(new AntlrInputStream(reader));
            return new CommonTokenStream(lexer);
        }

        #endregion
    }
}