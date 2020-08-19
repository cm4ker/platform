using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Parser;
using Xunit;

namespace Aquila.Compiler.Test2
{
    public class ParserTest
    {
        private ZLanguageVisitor _v;

        public ParserTest()
        {
            _v = new ZLanguageVisitor();
        }

        [Fact]
        public void LiteralExpressionTest()
        {
            var parser = Parse("10");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<Expression>(result);
        }

        [Fact]
        public void BinaryExpressionTest()
        {
            var parser = Parse("10 + 1");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<BinaryEx>(result);
        }

        [Fact]
        public void UnaryExpressionTest()
        {
            var parser = Parse("!a");
            var result = _v.VisitExpression(parser.expression());
            var expr = Assert.IsAssignableFrom<UnaryEx>(result);
            Assert.Equal(Operations.LogicNegation, expr.Operation);
        }

        [Fact]
        public void DeclareVariableTest()
        {
            var parser = Parse("int a = 10");
            var result = _v.VisitLocal_variable_declaration(parser.local_variable_declaration());
            var expr = Assert.IsAssignableFrom<VarDecl>(result);
        }

        [Fact]
        public void AssignTest()
        {
            var parser = Parse("a = 10");
            var result = _v.VisitAssignment(parser.assignment());
            var expr = Assert.IsAssignableFrom<AssignEx>(result);
        }

        [Fact]
        public void BlockTest()
        {
            var parser = Parse("{int a = 10; a = 20; {int c = 300;}}");
            var result = _v.VisitBlock(parser.block());
            var expr = Assert.IsAssignableFrom<BlockStmt>(result);
            Assert.Equal(3, expr.Statements.Count);
            Assert.IsAssignableFrom<BlockStmt>(expr.Statements[2]);
        }

        [Fact]
        public void IfTest()
        {
            var parser = Parse("if(true) a = 10 else a = 20");
            var result = _v.VisitEmbedded_statement(parser.embedded_statement());
            var expr = Assert.IsAssignableFrom<IfStmt>(result);
        }

        [Fact]
        public void MethodDeclarationTest()
        {
            var parser = Parse("public void A() {}");
            var result = _v.Visit(parser.method_declaration());
            var expr = Assert.IsAssignableFrom<MethodDecl>(result);
        }

        [Fact]
        public void CompilationUnitTest()
        {
            var parser = Parse("public void A() {} public void b() {}");
            var result = _v.Visit(parser.entryPoint());
            var expr = Assert.IsAssignableFrom<SourceUnit>(result);
            Assert.Equal(2, expr.Methods.Count);
        }

        #region Helpers

        private ZSharpParser Parse(string text)
        {
            return ParserHelper.Parse(text);
        }

        #endregion
    }
}