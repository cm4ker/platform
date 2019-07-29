using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using Class = ZenPlatform.Language.Ast.Definitions.Class;
using SyntaxNode = ZenPlatform.Language.Ast.SyntaxNode;
using TypeSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax;


namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public class VRoslyn : AstVisitorBase<Microsoft.CodeAnalysis.SyntaxNode>
    {
        private readonly CompilationOptions _opts;

        public VRoslyn(CompilationOptions opts)
        {
            _opts = opts;
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitLogicalOrArithmeticExpression(
            LogicalOrArithmeticExpression arg)
        {
            return arg.OperaotrType switch
            {
                UnaryOperatorType.Positive => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.UnaryPlusExpression,
                    (ExpressionSyntax) Visit(arg.Expression)),
                UnaryOperatorType.Negative => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.UnaryMinusExpression,
                    (ExpressionSyntax) Visit(arg.Expression)),
                UnaryOperatorType.Not => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression,
                    (ExpressionSyntax) Visit(arg.Expression)),
                _ => throw new Exception("Can't")
            };
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitCompilationUnit(CompilationUnit cu)
        {
            return SyntaxFactory.CompilationUnit()
                .AddUsings(cu.Namespaces.Select(GetUsing).ToArray())
                .AddMembers(cu.Entityes.Select(x => Visit(x)).Cast<MemberDeclarationSyntax>().ToArray());
        }

        private NameSyntax ParseName(string name)
        {
            return SyntaxFactory.ParseName(name);
        }


        public override Microsoft.CodeAnalysis.SyntaxNode VisitReturn(Return obj)
        {
//            if (obj.GetParent<Function>()?.Type is MultiTypeNode mt)
//                return SyntaxFactory.ReturnStatement(WrapUnionType(obj.Value, mt));
            return SyntaxFactory.ReturnStatement((ExpressionSyntax) Visit(obj.Expression));
        }

        private UsingDirectiveSyntax GetUsing(string name)
        {
            return SyntaxFactory.UsingDirective(ParseName(name));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitClass(Class obj)
        {
            return SyntaxFactory.ClassDeclaration(obj.Name)
                .WithMembers(NormolizeTypeBody(obj.TypeBody));
        }

        private SyntaxList<Microsoft.CodeAnalysis.SyntaxNode> NormolizeTypeBody(Language.Ast.Definitions.TypeBody tb)
        {
            return new SyntaxList<Microsoft.CodeAnalysis.SyntaxNode>()
                .AddRange(tb.Fields.Select(Visit).Cast<MemberDeclarationSyntax>())
                .AddRange(tb.Functions.Select(Visit).Cast<MemberDeclarationSyntax>())
                .AddRange(tb.Properties.Select(Visit).Cast<MemberDeclarationSyntax>());
        }

        private SyntaxList<StatementSyntax> GetStatements(Block node)
        {
            return new SyntaxList<StatementSyntax>(node.Statements.Select(x =>
            {
                var r = Visit(x);

                if (r is ExpressionSyntax es)
                    r = SyntaxFactory.ExpressionStatement(es);

                return r;
            }).Cast<StatementSyntax>());
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitVariable(Variable obj)
        {
            if (obj.Type is UnionTypeSyntax mt)
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

        private VariableDeclaratorSyntax GetVariabbleWithInit(string vName, SyntaxNode exp, bool mt = false,
            string varInit = null)
        {
            if (mt)
            {
                var mtType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
                var init = ((ExpressionSyntax) Visit(exp));

                return SyntaxFactory.VariableDeclarator(vName)
                    .WithInitializer(SyntaxFactory.EqualsValueClause(init));
            }


            return SyntaxFactory.VariableDeclarator(vName)
                .WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax) Visit(exp)));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitCastExpression(CastExpression obj)
        {
            return SyntaxFactory.CastExpression(GetTypeSyntax(obj.Type), (ExpressionSyntax) Visit(obj.Expression));
        }

        private TypeSyntax GetTypeSyntax(Language.Ast.Definitions.TypeSyntax tn)
        {
            return GetStandardType(tn) ?? SyntaxFactory.ParseTypeName(((SingleTypeSyntax) tn).TypeName);
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitLiteral(Literal obj)
        {
            return SyntaxFactory.LiteralExpression(GetLiteralKind(obj),
                GetLiteralSyntaxToken(obj));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitIf(If obj)
        {
            var @if = SyntaxFactory.IfStatement((ExpressionSyntax) Visit(obj.Condition),
                (BlockSyntax) Visit(obj.IfBlock));

            if (obj.ElseBlock != null)
                return @if.WithElse(SyntaxFactory.ElseClause((BlockSyntax) Visit(obj.ElseBlock)));
            return @if;
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitProperty(Property obj)
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

        private TypeSyntax GetStandardType(Language.Ast.Definitions.TypeSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case TypeNodeKind.String:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                case TypeNodeKind.Int: return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
                case TypeNodeKind.Double:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword));
                case TypeNodeKind.Boolean:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
                case TypeNodeKind.Char:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword));
                case TypeNodeKind.Void:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
                case TypeNodeKind.Object:
                case TypeNodeKind.UnionType:
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
                    ;
            }

            return null;
        }

        private SyntaxToken GetLiteralSyntaxToken(Literal node)
        {
            switch (node.Type.Kind)
            {
                case TypeNodeKind.String: return SyntaxFactory.Literal(node.Value);
                case TypeNodeKind.Int: return SyntaxFactory.Literal((int) node.ObjectiveValue);
                case TypeNodeKind.Double: return SyntaxFactory.Literal((double) node.ObjectiveValue);
                case TypeNodeKind.Boolean
                    when (bool) node.ObjectiveValue: return SyntaxFactory.Token(SyntaxKind.TrueKeyword);
                case TypeNodeKind.Boolean
                    when !(bool) node.ObjectiveValue: return SyntaxFactory.Token(SyntaxKind.FalseKeyword);
                case TypeNodeKind.Char: return SyntaxFactory.Literal((char) node.ObjectiveValue);
            }

            throw new Exception($"We can't process this literal kind {node.Type.Kind}");
        }

        private SyntaxKind GetLiteralKind(Literal node)
        {
            switch (node.Type.Kind)
            {
                case TypeNodeKind.String: return SyntaxKind.StringLiteralExpression;
                case TypeNodeKind.Int:
                case TypeNodeKind.Double: return SyntaxKind.NumericLiteralExpression;
                case TypeNodeKind.Boolean when (bool) node.ObjectiveValue: return SyntaxKind.TrueLiteralExpression;
                case TypeNodeKind.Boolean when !(bool) node.ObjectiveValue: return SyntaxKind.FalseLiteralExpression;
                case TypeNodeKind.Char: return SyntaxKind.CharacterLiteralExpression;
            }


            throw new Exception($"We can't process this literal kind {node.Type.Kind}");
        }


        public override Microsoft.CodeAnalysis.SyntaxNode VisitField(Field obj)
        {
            return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(GetTypeSyntax(obj.Type))
                    .AddVariables(SyntaxFactory.VariableDeclarator(obj.Name)));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitFunction(Function obj)
        {
            ParameterListSyntax pl = SyntaxFactory.ParameterList();

            foreach (var p in obj.Parameters)
            {
                pl = pl.AddParameters((ParameterSyntax) VisitParameter(p));
            }

            //Если мы компилируем для клиента, то на все вызывные функции необходимо вставить метод заглушку, котоырй будет дёргать клиента
            if (obj.Flags == FunctionFlags.ServerClientCall && _opts.Mode == CompilationMode.Client)
            {
                var invokeExpr = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ParseName("GlobalScope"), SyntaxFactory.IdentifierName("Client"));

                var block = SyntaxFactory.Block()
                    .AddStatements(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    invokeExpr, SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("Invoke"),
                                        SyntaxFactory.TypeArgumentList()
                                            .AddArguments(GetTypeSyntax(obj.Type))
                                            .AddArguments(SyntaxFactory.ParseTypeName("object[]"))))
                            ).AddArgumentListArguments(
                                obj.Parameters.Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name)))
                                    .ToArray()
                            )));

                if (obj.Type.Kind != TypeNodeKind.Void)
                    block = block.ReplaceNode(block.Statements[0], SyntaxFactory.ReturnStatement(
                        ((ExpressionStatementSyntax) block.Statements[0]).Expression));

                return SyntaxFactory.MethodDeclaration(GetTypeSyntax(obj.Type), obj.Name)
                    .WithBody(block)
                    .WithParameterList(pl);
            }

            return SyntaxFactory.MethodDeclaration(GetTypeSyntax(obj.Type), obj.Name)
                .WithBody((BlockSyntax) Visit(obj.Block))
                .WithParameterList(pl);
        }


        public override Microsoft.CodeAnalysis.SyntaxNode VisitInstructionsBody(Block obj)
        {
            return SyntaxFactory.Block(GetStatements(obj));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitModule(Module obj)
        {
            return SyntaxFactory.ClassDeclaration(obj.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .WithMembers(NormolizeTypeBody(obj.TypeBody));
        }

        private SeparatedSyntaxList<ExpressionSyntax> EmptyList = new SeparatedSyntaxList<ExpressionSyntax>();

        public override Microsoft.CodeAnalysis.SyntaxNode VisitAssignment(Assignment obj)
        {
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression, ParseName(obj.Name.Value),
                (ExpressionSyntax) Visit(obj.Value)));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitParameter(Parameter obj)
        {
            return SyntaxFactory.Parameter(SyntaxFactory.Identifier(obj.Name)).WithType(GetTypeSyntax(obj.Type));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitFor(For obj)
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
                (BlockSyntax) Visit(obj.Block));
        }


        public override Microsoft.CodeAnalysis.SyntaxNode VisitPostIncrementExpression(PostIncrementExpression obj)
        {
            return SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression,
                (ExpressionSyntax) Visit(obj.Name));
        }

        public override Microsoft.CodeAnalysis.SyntaxNode VisitName(Name obj)
        {
            if (obj.Type is UnionTypeSyntax)
            {
                return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    ParseName(obj.Value), SyntaxFactory.IdentifierName("Value"));
            }

            return SyntaxFactory.IdentifierName(obj.Value);
        }


        public override Microsoft.CodeAnalysis.SyntaxNode VisitBinaryExpression(BinaryExpression obj)
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
                BinaryOperatorType.GreaterThen => SyntaxKind.GreaterThanExpression,
                _ => throw new Exception("This syntax kind not supported")
            };
            return SyntaxFactory.ParenthesizedExpression(SyntaxFactory.BinaryExpression(kind, left, right));
        }
    }
}