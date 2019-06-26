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
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
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
            if (obj.GetParent<Function>()?.Type is MultiTypeNode mt)
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
            return new SyntaxList<SyntaxNode>()
                .AddRange(tb.Fields.Select(Visit).Cast<MemberDeclarationSyntax>())
                .AddRange(tb.Functions.Select(Visit).Cast<MemberDeclarationSyntax>())
                .AddRange(tb.Properties.Select(Visit).Cast<MemberDeclarationSyntax>());
        }

        private SyntaxList<StatementSyntax> GetStatements(InstructionsBodyNode node)
        {
            return new SyntaxList<StatementSyntax>(node.Statements.Select(x =>
            {
                var r = Visit(x);

                if (r is ExpressionSyntax es)
                    r = SyntaxFactory.ExpressionStatement(es);
                return r;
            }).Cast<StatementSyntax>());
        }

        public override SyntaxNode VisitVariable(Variable obj)
        {
            if (obj.Type is MultiTypeNode mt)
            {
                //мы должны обернуть все
                return SyntaxFactory.LocalDeclarationStatement(SyntaxFactory
                    .VariableDeclaration(GetTypeSyntax(obj.Type))
                    .AddVariables(GetVariabbleWithInit(obj.Name, obj.Value, true, mt.DeclName)));
            }

            return SyntaxFactory.LocalDeclarationStatement(SyntaxFactory
                .VariableDeclaration(GetTypeSyntax(obj.Type))
                .AddVariables(GetVariabbleWithInit(obj.Name, obj.Value)));
        }

        private VariableDeclaratorSyntax GetVariabbleWithInit(string vName, AstNode exp, bool mt = false,
            string varInit = null)
        {
            if (mt)
            {
                var mtType = SyntaxFactory.IdentifierName("MultiTypeDataStorage");
                var init = SyntaxFactory.ObjectCreationExpression(mtType)
                    .WithArgumentList(SyntaxFactory.ArgumentList()
                        .AddArguments(
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName("MT_1"))
                            , SyntaxFactory.Argument((ExpressionSyntax) Visit(exp))));

                return SyntaxFactory.VariableDeclarator(vName)
                    .WithInitializer(SyntaxFactory.EqualsValueClause(init));
            }


            return SyntaxFactory.VariableDeclarator(vName)
                .WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax) Visit(exp)));
        }

        public override SyntaxNode VisitCastExpression(CastExpression obj)
        {
            return SyntaxFactory.CastExpression(GetTypeSyntax(obj.Type), (ExpressionSyntax) Visit(obj.Value));
        }

        private TypeSyntax GetTypeSyntax(TypeNode tn)
        {
            return GetStandardType(tn) ?? SyntaxFactory.ParseTypeName(tn.Type.Name);
        }

        public override SyntaxNode VisitLiteral(Literal obj)
        {
            return SyntaxFactory.LiteralExpression(GetLiteralKind(obj),
                GetLiteralSyntaxToken(obj));
        }


        public override SyntaxNode VisitProperty(Property obj)
        {
            AccessorDeclarationSyntax getAccessor, setAccessor;

            if (obj.Getter != null)
                getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration,
                    (BlockSyntax) Visit(obj.Getter));
            else
                getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));


            if (obj.Setter != null)
                setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration,
                    (BlockSyntax) Visit(obj.Setter));
            else
                setAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return SyntaxFactory.PropertyDeclaration(GetTypeSyntax(obj.Type), obj.Name)
                .AddAccessorListAccessors(getAccessor)
                .AddAccessorListAccessors(setAccessor);
        }

        private TypeSyntax GetStandardType(TypeNode node)
        {
            switch (node.Type.Name)
            {
                case "String": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                case "Int32": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
                case "Double": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword));
                case "Bool": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
                case "Char": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword));
                case "Void": return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
            }

            return null;
        }

        private SyntaxToken GetLiteralSyntaxToken(Literal node)
        {
            switch (node.Type.Type.Name)
            {
                case "String": return SyntaxFactory.Literal(node.Value);
                case "Int32": return SyntaxFactory.Literal((int) node.ObjectiveValue);
                case "Double": return SyntaxFactory.Literal((double) node.ObjectiveValue);
                case "Bool" when (bool) node.ObjectiveValue: return SyntaxFactory.Token(SyntaxKind.TrueKeyword);
                case "Bool" when !(bool) node.ObjectiveValue: return SyntaxFactory.Token(SyntaxKind.FalseKeyword);
                case "Char": return SyntaxFactory.Literal((char) node.ObjectiveValue);
            }

            throw new Exception($"We can't process this literal kind {node.Type.Type.Name}");
        }

        private SyntaxKind GetLiteralKind(Literal node)
        {
            switch (node.Type.Type.Name)
            {
                case "String": return SyntaxKind.StringLiteralExpression;
                case "Int32":
                case "Double": return SyntaxKind.NumericLiteralExpression;
                case "Bool" when (bool) node.ObjectiveValue: return SyntaxKind.TrueLiteralExpression;
                case "Bool" when !(bool) node.ObjectiveValue: return SyntaxKind.FalseLiteralExpression;
                case "Char": return SyntaxKind.CharacterLiteralExpression;
            }


            throw new Exception($"We can't process this literal kind {node.Type.Type.Name}");
        }


        public override SyntaxNode VisitField(Field obj)
        {
            return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(GetTypeSyntax(obj.Type))
                    .AddVariables(SyntaxFactory.VariableDeclarator(obj.Name)));
        }

        public override SyntaxNode VisitFunction(Function obj)
        {
            ParameterListSyntax pl = SyntaxFactory.ParameterList();

            foreach (var p in obj.Parameters)
            {
                pl = pl.AddParameters((ParameterSyntax) VisitParameter(p));
            }

            return SyntaxFactory.MethodDeclaration(GetTypeSyntax(obj.Type), obj.Name)
                .WithBody((BlockSyntax) Visit(obj.InstructionsBody))
                .WithParameterList(pl);
        }

        public override SyntaxNode VisitInstructionsBody(InstructionsBodyNode obj)
        {
            return SyntaxFactory.Block(GetStatements(obj));
        }

        public override SyntaxNode VisitModule(Module obj)
        {
            return SyntaxFactory.ClassDeclaration(obj.Name);
        }

        private SeparatedSyntaxList<ExpressionSyntax> EmptyList = new SeparatedSyntaxList<ExpressionSyntax>();

        public override SyntaxNode VisitAssigment(Assignment obj)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, ParseName(obj.Name),
                (ExpressionSyntax) Visit(obj.Value)));
        }

        public override SyntaxNode VisitParameter(Parameter obj)
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier(obj.Name)).WithType(GetTypeSyntax(obj.Type));
        }

        public override SyntaxNode VisitFor(For obj)
        {
            VariableDeclarationSyntax vTypeSyntax = null;
            List<ExpressionSyntax> initializers;

            if (obj.Initializer is Variable v)
            {
                vTypeSyntax = SyntaxFactory.VariableDeclaration(GetTypeSyntax(v.Type))
                    .AddVariables(GetVariabbleWithInit(v.Name, v.Value));
            }

            var inc = SyntaxFactory.SeparatedList<ExpressionSyntax>().Add((ExpressionSyntax) Visit(obj.Counter));

            return SyntaxFactory.ForStatement(vTypeSyntax,
                EmptyList,
                (ExpressionSyntax) Visit(obj.Condition),
                inc,
                (BlockSyntax) Visit(obj.InstructionsBody));
        }

        public override SyntaxNode VisitPostIncrementStatement(PostIncrementStatement obj)
        {
            return SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression,
                (ExpressionSyntax) Visit(obj.Name));
        }

        public override SyntaxNode VisitName(Name obj)
        {
            if (obj.Type is MultiTypeNode)
            {
                return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    ParseName(obj.Value), SyntaxFactory.IdentifierName("Value"));
            }


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
                BinaryOperatorType.LessThen => SyntaxKind.LessThanExpression,
                _ => throw new Exception("Can't")
                };
            return SyntaxFactory.ParenthesizedExpression(SyntaxFactory.BinaryExpression(kind, left, right));
        }
    }
}