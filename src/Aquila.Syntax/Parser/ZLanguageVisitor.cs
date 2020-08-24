using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Text;
using MoreLinq.Extensions;

namespace Aquila.Syntax.Parser
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<LangElement>
    {
        private Stack<SyntaxStack> _stackOfStacks;

        private SyntaxStack Stack => _stackOfStacks.Peek();


        public ZLanguageVisitor()
        {
            _stackOfStacks = new Stack<SyntaxStack>();
            PushStack();
        }

        private void PushStack()
        {
            _stackOfStacks.Push(new SyntaxStack());
        }

        private SyntaxStack PopStack()
        {
            return _stackOfStacks.Pop();
        }

        public override LangElement VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            //PushStack();
            //
            // var usings = new UsingList();
            //
            // foreach (var atd in context.usingSection())
            // {
            //     usings.Add((UsingBase) Visit(atd));
            // }
            //
            PushStack();
            context.method_declaration().ForEach(x => VisitMethod_declaration(x));
            var methods = PopStack();

            var cu = new SourceUnit(context.ToLineInfo(), SyntaxKind.CompilationUnit,
                "C:\\Users\\cmake\\source\\repos\\ZenPlatform\\src\\Aquila.Compiler.Tests\\bin\\Debug\\netcoreapp3.1\\test.aq",
                UsingList.Empty,
                methods.ToCollection<MethodList>(), FieldList.Empty);

            PopStack();

            return cu;
        }

        // public override LangElement VisitAliasingTypeDefinition(ZSharpParser.AliasingTypeDefinitionContext context)
        // {
        //     return new UsingAliasDecl(context.ToLineInfo(), SyntaxKind.BadToken,
        //         context.typeName().GetText(),
        //         context.alias.GetText());
        // }

        public override LangElement VisitUsingDefinition(ZSharpParser.UsingDefinitionContext context)
        {
            base.VisitUsingDefinition(context);
            return new UsingDecl(context.ToLineInfo(), SyntaxKind.UsingDerictive,
                context.@namespace().GetText());
        }

        // public override LangElement VisitFieldDeclaration(ZSharpParser.FieldDeclarationContext context)
        // {
        //     base.VisitFieldDeclaration(context);
        //     FieldDecl f = new FieldDecl(context.ToLineInfo(), SyntaxKind.FieldDeclaration,
        //         Stack.PopIdentifier(),
        //         Stack.PopType());
        //     Stack.PeekCollection().Add(f);
        //     return f;
        // }

        // public override LangElement VisitAttributes(ZSharpParser.AttributesContext context)
        // {
        //     Stack.Push(new AnnotationList());
        //     base.VisitAttributes(context);
        //
        //     return null;
        // }
        //
        // public override LangElement VisitAttribute(ZSharpParser.AttributeContext context)
        // {
        //     base.VisitAttribute(context);
        //
        //     ArgumentList ac = null;
        //
        //     if (context.argument_list() != null)
        //         ac = (ArgumentList) Stack.Pop();
        //
        //     var result =
        //         new Annotation(context.ToLineInfo(), SyntaxKind.Attribute, ac, null);
        //     Stack.PeekType<AnnotationList>().Add(result);
        //
        //     return result;
        // }

        public override LangElement VisitReturn_type(ZSharpParser.Return_typeContext context)
        {
            base.VisitReturn_type(context);

            if (context.VOID() != null)
                Stack.Push(TypeSyntaxHelper.CreateVoid(context.ToLineInfo()));

            return Stack.PeekNode();
        }

        public override LangElement VisitType_(ZSharpParser.Type_Context context)
        {
            var result = TypeSyntaxHelper.Create(context.ToLineInfo(), context.GetText());
            Stack.Push(result);
            return result;
        }

        // public override LangElement VisitArrayType(ZSharpParser.ArrayTypeContext context)
        // {
        //     base.VisitArrayType(context);
        //
        //     var result = new ArrayType(context.ToLineInfo(), SyntaxKind.ArrayType, Stack.PopType());
        //
        //     Stack.Push(result);
        //     return result;
        // }

        public override LangElement VisitLiteral(ZSharpParser.LiteralContext context)
        {
            LiteralEx result = null;
            Span li = context.ToLineInfo();
            if (context.string_literal() != null)
            {
                //Строки парсятся сюда вместе с кавычками и чтобы их убрать приходится
                //заниматься таким вот извращением

                var text = context.string_literal().REGULAR_STRING()?.GetText() ??
                           context.string_literal().VERBATIUM_STRING()?.GetText();

                text = Regex.Unescape(text ?? throw new NullReferenceException());

                if (context.string_literal().REGULAR_STRING() != null)
                    result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.StringLiteral,
                        text.Substring(1, text.Length - 2), false);
                else
                    result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.StringLiteral,
                        text.Substring(2, text.Length - 3), false);

                result.ObjectiveValue = result.Value;
            }
            else if (context.boolean_literal() != null)
            {
                result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.BoolLiteral, context.GetText(),
                    false);
                result.ObjectiveValue = bool.Parse(result.Value);
            }
            else if (context.INTEGER_LITERAL() != null)
            {
                result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.LongIntLiteral, context.GetText(),
                    false);
                result.ObjectiveValue = int.Parse(result.Value);
            }
            else if (context.REAL_LITERAL() != null)
            {
                result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.DoubleLiteral, context.GetText(),
                    false);
                result.ObjectiveValue = double.Parse(result.Value);
            }
            else if (context.CHARACTER_LITERAL() != null)
            {
                result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.CharLiteral,
                    context.GetText().Substring(1, 1), false);
                result.ObjectiveValue = result.Value[0];
            }

            //TODO: Не обработанным остался HEX INTEGER LITERAL его необходимо доделать

            if (result == null)
                throw new Exception("Unknown literal");

            Stack.Push(result);

            return result;
        }

        public override LangElement VisitIdentifier(ZSharpParser.IdentifierContext context)
        {
            Stack.Push(new IdentifierToken(context.ToLineInfo(), SyntaxKind.IdentifierToken, context.GetText()));
            return Stack.PeekNode();
        }

        public override LangElement VisitLocal_variable_declarator(
            ZSharpParser.Local_variable_declaratorContext context)
        {
            base.VisitLocal_variable_declarator(context);

            LangElement decl;

            if (context.local_variable_initializer() != null)
                decl = new VarDeclarator(context.ToLineInfo(), SyntaxKind.VariableDeclarator,
                    Stack.PopExpression(), Stack.PopIdentifier());
            else
                decl = new VarDeclarator(context.ToLineInfo(), SyntaxKind.VariableDeclarator,
                    null, Stack.PopIdentifier());

            Stack.Push(decl);

            return Stack.PeekNode();
        }

        public override LangElement VisitLocal_variable_declaration(
            ZSharpParser.Local_variable_declarationContext context)
        {
            base.VisitLocal_variable_type(context.local_variable_type());

            DeclaratorList list;

            {
                PushStack();
                foreach (var declarator in context.local_variable_declarator())
                {
                    VisitLocal_variable_declarator(declarator);
                }

                var declarators = PopStack();
                list = declarators.ToCollection<DeclaratorList>();
            }

            Stack.Push(new VarDecl(context.ToLineInfo(), SyntaxKind.VariableDeclaration,
                Stack.PopType(), list));

            return Stack.PeekNode();
        }

        // public override LangElement VisitCastExpression(ZSharpParser.CastExpressionContext context)
        // {
        //     base.VisitCastExpression(context);
        //
        //     var result = new CastEx(context.ToLineInfo(), SyntaxKind.CastExpression,
        //         Operations.ObjectCast, Stack.PopExpression(), Stack.PopType());
        //
        //     Stack.Push(result);
        //
        //     return result;
        // }

        public override LangElement VisitMethod_declaration(ZSharpParser.Method_declarationContext context)
        {
            base.VisitMethod_declaration(context);

            MethodDecl result = null;
            ParameterList pc = new ParameterList();
            AnnotationList ac = new AnnotationList();
            //GenericParameterList gpc = new GenericParameterList();

            var body = Stack.PopInstructionsBody();

            // if (context.genericParameters() != null)
            // {
            //     gpc = (GenericParameterList) Stack.Pop();
            // }

            if (context.parameters() != null)
            {
                pc = (ParameterList) Stack.Pop();
            }

            var type = Stack.PopType();

            var funcName = context.IDENTIFIER().Identifier();

            // if (context.attributes() != null)
            // {
            //     ac = (AnnotationList) Stack.Pop();
            // }

            Stack.Push(new MethodDecl(context.ToLineInfo(), SyntaxKind.MethodDeclaration, body, pc, //gpc,
                ac,
                funcName, type));

            return Stack.PeekNode();
        }

        public override LangElement VisitParameters(ZSharpParser.ParametersContext context)
        {
            Stack.Push(new ParameterList());
            return base.VisitParameters(context);
        }

        public override LangElement VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = Stack.PeekType<ParameterList>();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new Parameter(context.ToLineInfo(), SyntaxKind.Parameter,
                Stack.PopType(), context.IDENTIFIER().Identifier(), passMethod);

            paramList.Add(parameter);

            return null;
        }

        public override LangElement VisitBlock(ZSharpParser.BlockContext context)
        {
            PushStack();
            base.VisitBlock(context);
            var sttmts = PopStack();
            Stack.Push(new BlockStmt(context.ToLineInfo(), SyntaxKind.BlockStatement,
                sttmts.ToCollection<StatementList>()));
            return Stack.PeekNode();
        }

        public override LangElement VisitArgument_list(ZSharpParser.Argument_listContext context)
        {
            PushStack();
            base.VisitArgument_list(context);
            var args = PopStack().ToCollection<ArgumentList>();
            Stack.Push(args);

            return Stack.PeekNode();
        }

        public override LangElement VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new Argument(context.ToLineInfo(), SyntaxKind.Argument, Stack.PopExpression(),
                passMethod);

            Stack.Push(result);

            return Stack.PeekNode();
        }

        public override LangElement VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);
            return Stack.PeekNode();
        }

        // public override LangElement VisitExpressionPostfix(ZSharpParser.ExpressionPostfixContext context)
        // {
        //     base.VisitExpressionPostfix(context);
        //
        //     if (context.indexerExpression != null)
        //         Stack.Push(new IndexerEx(context.ToLineInfo(), SyntaxKind.IndexerExpression,
        //             Operations.Indexer,
        //             Stack.PopExpression(),
        //             Stack.PopExpression()));
        //
        //     return null;
        // }

        public override LangElement VisitUnary_expression(ZSharpParser.Unary_expressionContext context)
        {
            base.VisitUnary_expression(context);

            var li = context.ToLineInfo();

            if (context.PLUS() != null)
                Stack.Push(new UnaryEx(li, SyntaxKind.PlusToken, Operations.Plus, Stack.PopExpression()));
            if (context.MINUS() != null)
                Stack.Push(new UnaryEx(li, SyntaxKind.MinusToken, Operations.Minus, Stack.PopExpression()));
            if (context.BANG() != null)
                Stack.Push(new UnaryEx(li, SyntaxKind.BangToken, Operations.LogicNegation, Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitMultiplicative_expression(
            ZSharpParser.Multiplicative_expressionContext context)
        {
            base.VisitMultiplicative_expression(context);

            if (context.range_expression().Length > 1)
            {
                Operations opType;

                if (context.PERCENT() != null) opType = Operations.Mod;
                else if (context.DIV() != null) opType = Operations.Div;
                else if (context.STAR() != null) opType = Operations.Mul;
                else throw new Exception();

                Stack.Push(new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    opType, Stack.PopExpression(),
                    Stack.PopExpression()));
            }

            return null;
        }

        public override LangElement VisitEquality_expression(ZSharpParser.Equality_expressionContext context)
        {
            base.VisitEquality_expression(context);

            if (context.relational_expression().Length > 1)
            {
                Operations opType;

                if (context.OP_EQ() != null) opType = Operations.Equal;
                else if (context.OP_NE() != null) opType = Operations.NotEqual;
                else throw new Exception();
                Stack.Push(new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    opType, Stack.PopExpression(),
                    Stack.PopExpression()));
            }

            return null;
        }

        public override LangElement VisitRelational_expression(ZSharpParser.Relational_expressionContext context)
        {
            base.VisitRelational_expression(context);

            if (context.shift_expression().Length > 1)
            {
                Operations opType = Operations.Unknown;

                if (context.GT() != null) opType = Operations.GreaterThan;
                if (context.LT() != null) opType = Operations.LessThan;
                if (context.OP_GE() != null) opType = Operations.GreaterThanOrEqual;
                if (context.OP_LE() != null) opType = Operations.LessThanOrEqual;

                Stack.Push(new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression, opType,
                    Stack.PopExpression(),
                    Stack.PopExpression()));
            }

            return null;
        }

        public override LangElement VisitAdditive_expression(ZSharpParser.Additive_expressionContext context)
        {
            base.VisitAdditive_expression(context);

            if (context.multiplicative_expression().Length > 1)
            {
                Operations opType = Operations.Unknown;

                if (context.PLUS().Any()) opType = Operations.Add;
                else if (context.MINUS().Any()) opType = Operations.Sub;

                var result = new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression, opType,
                    Stack.PopExpression(),
                    Stack.PopExpression());

                Stack.Push(result);
            }


            return null;
        }

        public override LangElement VisitConditional_or_expression(
            ZSharpParser.Conditional_or_expressionContext context)
        {
            base.VisitConditional_or_expression(context);

            if (context.OP_OR()?.Any() ?? false)
                Stack.Push(
                    new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression, Operations.Or,
                        Stack.PopExpression(),
                        Stack.PopExpression()));

            return Stack.PeekNode();
        }


        public override LangElement VisitConditional_and_expression(
            ZSharpParser.Conditional_and_expressionContext context)
        {
            base.VisitConditional_and_expression(context);

            if (context.OP_AND()?.Any() ?? false)
                Stack.Push(
                    new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression, Operations.And,
                        Stack.PopExpression(),
                        Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitMethod_invocation(ZSharpParser.Method_invocationContext context)
        {
            base.VisitMethod_invocation(context);

            Stack.Push(new CallEx(context.ToLineInfo(), SyntaxKind.CallExpression, Operations.Call,
                Stack.Pop<ArgumentList>(), Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitMember_access(ZSharpParser.Member_accessContext context)
        {
            base.VisitMember_access(context);
            
            Stack.Push(new MemberAccessEx(context.ToLineInfo(), SyntaxKind.MemberAccessExpression,
                Operations.MemberAccess, Stack.PopIdentifier(), Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitAssignment(ZSharpParser.AssignmentContext context)
        {
            base.VisitAssignment(context);

            Stack.Push(new AssignEx(context.ToLineInfo(), SyntaxKind.EqualsToken, Operations.AssignValue,
                Stack.PopExpression(), Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitThrowStatement(ZSharpParser.ThrowStatementContext context)
        {
            // base.VisitThrowStatement(context);
            //
            // var result = new Throw(context.ToLineInfo(), Stack.PopExpression());
            // Stack.Push(result);
            //
            // return result;

            return null;
        }

        public override LangElement VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            Statement @else = null;

            if (context.ELSE() != null)
            {
                @else = Stack.PopStatement();
            }

            var result = new IfStmt(context.ToLineInfo(), SyntaxKind.IfStatement, @else, Stack.PopStatement(),
                Stack.PopExpression());

            Stack.Push(result);

            return result;
        }

        public override LangElement VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new ForStmt(context.ToLineInfo(), SyntaxKind.ForStatement, Stack.PopInstructionsBody(),
                Stack.PopExpression(),
                Stack.PopExpression(), Stack.PopExpression());

            Stack.Push(result);

            return result;
        }

        public override LangElement VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new WhileStmt(context.ToLineInfo(), SyntaxKind.WhileStatement, Stack.PopInstructionsBody(),
                Stack.PopExpression());
            Stack.Push(result);

            return result;
        }

        public override LangElement VisitExpressionStatement(ZSharpParser.ExpressionStatementContext context)
        {
            base.VisitExpressionStatement(context);

            Stack.Push(new ExpressionStmt(context.ToLineInfo(), SyntaxKind.ExpressionStatement,
                Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override LangElement VisitReturnStatement(ZSharpParser.ReturnStatementContext context)
        {
            base.VisitReturnStatement(context);

            Stack.Push(new ReturnStmt(context.ToLineInfo(), SyntaxKind.ReturnStatement, Stack.PopExpression()));

            return Stack.PeekNode();
        }
    }
}