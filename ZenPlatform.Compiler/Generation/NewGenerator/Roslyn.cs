using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Host.Mef;
using Mono.CompilerServices.SymbolWriter;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public class Class
    {
        public Class()
        {
        }

        public bool IsStatic { get; set; }

        public string Name { get; set; }
    }


    public class Infrastructure
    {
        private AdhocWorkspace _workspace;
        private SyntaxGenerator _generator;
        private readonly List<Class> _classes;

        public Infrastructure()
        {
            _workspace = new AdhocWorkspace();
            _generator = SyntaxGenerator.GetGenerator(_workspace, LanguageNames.CSharp);
            _classes = new List<Class>();
        }

        IReadOnlyList<Class> Classes => _classes;

        public Class DefineClass(string name)
        {
            var cls = new Class();
            _classes.Add(cls);
            return cls;
        }

        public void Build()
        {
        }
    }


    public class VRoslyn : AstVisitorBase<SyntaxNode>
    {
        public VRoslyn()
        {
        }

        public override SyntaxNode VisitLogicalOrArithmeticExpression(LogicalOrArithmeticExpression arg)
        {
            return arg.Type switch
                {
                UnaryOperatorType.Positive => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.UnaryPlusExpression,
                    (ExpressionSyntax) Visit(arg.Value)),
                UnaryOperatorType.Negative => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.UnaryMinusExpression,
                    (ExpressionSyntax) Visit(arg.Value)),
                UnaryOperatorType.Not => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression,
                    (ExpressionSyntax) Visit(arg.Value)),
                _ => throw new Exception("Can't")
                };
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnit cu)
        {
            return SyntaxFactory.CompilationUnit()
                .AddUsings(cu.Namespaces.Select(GetUsing).ToArray())
                .AddMembers(cu.TypeEntities.Select(x => Visit(x)).Cast<MemberDeclarationSyntax>().ToArray());
        }

        private NameSyntax ParseName(string name)
        {
            return SyntaxFactory.ParseName(name);
        }

        private ExpressionSyntax WrapMultitype(Expression exp, MultiTypeNode mtn)
        {
            var args = SyntaxFactory.ArgumentList().AddArguments(
                SyntaxFactory.Argument(ParseName("a")),
                SyntaxFactory.Argument((ExpressionSyntax) Visit(exp)),
                SyntaxFactory.Argument(ParseName("c"))
            );

            return SyntaxFactory.ObjectCreationExpression(ParseName(mtn.Type.Name), args, null);
        }

        public override SyntaxNode VisitReturn(Return obj)
        {
            if (obj.GetParent<Function>().Type is MultiTypeNode mt)
                return SyntaxFactory.ReturnStatement(WrapMultitype(obj.Value, mt));
            return SyntaxFactory.ReturnStatement((ExpressionSyntax) Visit(obj.Value));
        }

        private UsingDirectiveSyntax GetUsing(string name)
        {
            return SyntaxFactory.UsingDirective(ParseName(name));
        }

        public override SyntaxNode VisitClass(Language.Ast.AST.Definitions.Class obj)
        {
            return SyntaxFactory.ClassDeclaration(obj.Name)
                .WithMembers(NormolizeTypeBody(obj.TypeBody));
        }

        private SyntaxList<SyntaxNode> NormolizeTypeBody(TypeBody tb)
        {
            return new SyntaxList<SyntaxNode>(tb.Functions.Select(Visit).Cast<MemberDeclarationSyntax>());
        }

        private SyntaxList<StatementSyntax> GetStatements(InstructionsBodyNode node)
        {
            return new SyntaxList<StatementSyntax>(node.Statements.Select(x => Visit(x)).Cast<StatementSyntax>());
        }

        private TypeSyntax GetTypeSyntax(TypeNode tn)
        {
            return SyntaxFactory.ParseTypeName(tn.Type.Name);
        }

        public override SyntaxNode VisitLiteral(Literal obj)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(obj.Value));
        }

        public override SyntaxNode VisitFunction(Function obj)
        {
            return SyntaxFactory.MethodDeclaration(GetTypeSyntax(obj.Type), obj.Name)
                .WithBody((BlockSyntax) Visit(obj.InstructionsBody));
        }

        public override SyntaxNode VisitInstructionsBody(InstructionsBodyNode obj)
        {
            return SyntaxFactory.Block(GetStatements(obj));
        }

        public override SyntaxNode VisitModule(Module obj)
        {
            return SyntaxFactory.ClassDeclaration(obj.Name);
        }

        public override SyntaxNode VisitName(Name obj)
        {
            return SyntaxFactory.IdentifierName(obj.Value);
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpression obj)
        {
            var left = (ExpressionSyntax) Visit(obj.Left);
            var right = (ExpressionSyntax) Visit(obj.Right);

            var kind = obj.BinaryOperatorType switch
                {
                BinaryOperatorType.Add => SyntaxKind.AddExpression,
                BinaryOperatorType.Subtract => SyntaxKind.SubtractExpression,
                BinaryOperatorType.Divide => SyntaxKind.DivideExpression,
                BinaryOperatorType.Multiply => SyntaxKind.MultiplyExpression,
                BinaryOperatorType.Or => SyntaxKind.LogicalOrExpression,
                _ => throw new Exception("Can't")
                };
            return SyntaxFactory.ParenthesizedExpression(SyntaxFactory.BinaryExpression(kind, left, right));
        }
    }
}