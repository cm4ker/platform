using System;
using System.Linq;
using Aquila.Syntax;
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
        public void CallExpressionTest()
        {
            var parser = Parse("SimpleCall()");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<CallEx>(result);

            parser = Parse("SimpleCall(a, b, c)");
            result = _v.VisitExpression(parser.expression());

            var call = Assert.IsAssignableFrom<CallEx>(result);
            Assert.Equal(3, call.Arguments.Count);
        }


        [Fact]
        public void MemberAccessTest()
        {
            var parser = Parse("A.B");
            var result = _v.VisitExpression(parser.expression());
            var ma = Assert.IsAssignableFrom<MemberAccessEx>(result);
            Assert.Equal("B", ma.Identifier.Text);
            var lookup = Assert.IsAssignableFrom<NameEx>(ma.Expression);
            Assert.Equal("A", lookup.Identifier.Text);
        }

        [Fact]
        public void BinaryExpressionTest()
        {
            var parser = Parse("10 + 1");
            var result = _v.VisitExpression(parser.expression());
            Assert.IsAssignableFrom<BinaryEx>(result);
        }

        [Fact]
        public void MethodDecl2Test()
        {
            var parser = Parse(@"int CustomProc() 
            { 
                return 2 * 3 / 3 * 6 * (10 + 2); 
            }");
            var result = _v.Visit(parser.method_declaration());
            var expr = Assert.IsAssignableFrom<MethodDecl>(result);
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
            var parser = Parse("void A() {}");
            var result = _v.Visit(parser.method_declaration());
            var expr = Assert.IsAssignableFrom<MethodDecl>(result);
        }

        [Fact]
        public void CompilationUnitTest()
        {
            var parser = Parse("void A() {} void b() {}");
            var result = _v.Visit(parser.entryPoint());
            var expr = Assert.IsAssignableFrom<SourceUnit>(result);
            Assert.Equal(2, expr.Methods.Count);
        }

        [Fact]
        public void VarTypeTest()
        {
            var parser = Parse("var a = 10;");
            var result = _v.Visit(parser.statement());
            var expr = Assert.IsAssignableFrom<VarDecl>(result);
            Assert.True(expr.VariableType.IsVar);
        }


        [Fact]
        public void MissingElementTest()
        {
            var parser = Parse("var a = ;");
            var result = _v.Visit(parser.statement());
            var expr = Assert.IsAssignableFrom<VarDecl>(result);
            Assert.NotEmpty(expr.Declarators);
            var missing = Assert.IsType<MissingEx>(expr.Declarators[0].Initializer);
            Assert.NotEqual("", missing.Message);
        }


        #region Helpers

        private ZSharpParser Parse(string text)
        {
            return ParserHelper.Parse(text);
        }

        #endregion
    }
}