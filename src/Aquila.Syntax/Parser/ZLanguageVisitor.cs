using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;
using MoreLinq;

namespace Aquila.Compiler.Parser
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<SyntaxNode>
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

        public override SyntaxNode VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            PushStack();

            var usings = new UsingList();

            foreach (var atd in context.usingSection())
            {
                usings.Add((UsingBase) Visit(atd));
            }

            PushStack();
            context.method_declaration().ForEach(x => VisitMethod_declaration(x));
            var methods = PopStack();

            var cu = new CompilationUnitSyntax(context.ToLineInfo(), SyntaxKind.CompilationUnit, usings,
                methods.ToCollection<MethodList>(), FieldList.Empty);

            PopStack();

            return cu;
        }

        public override SyntaxNode VisitAliasingTypeDefinition(ZSharpParser.AliasingTypeDefinitionContext context)
        {
            return new UsingAliasDeclarationSyntax(context.ToLineInfo(), SyntaxKind.BadToken,
                context.typeName().GetText(),
                context.alias.GetText());
        }

        public override SyntaxNode VisitUsingDefinition(ZSharpParser.UsingDefinitionContext context)
        {
            base.VisitUsingDefinition(context);
            return new UsingDeclarationSyntax(context.ToLineInfo(), SyntaxKind.UsingDerictive,
                context.@namespace().GetText());
        }

        public override SyntaxNode VisitFieldDeclaration(ZSharpParser.FieldDeclarationContext context)
        {
            base.VisitFieldDeclaration(context);
            FieldDeclarationSyntax f = new FieldDeclarationSyntax(context.ToLineInfo(), SyntaxKind.FieldDeclaration,
                Stack.PopIdentifier(),
                Stack.PopType());
            Stack.PeekCollection().Add(f);
            return f;
        }

        public override SyntaxNode VisitAttributes(ZSharpParser.AttributesContext context)
        {
            Stack.Push(new AttributeList());
            base.VisitAttributes(context);

            return null;
        }

        public override SyntaxNode VisitAttribute(ZSharpParser.AttributeContext context)
        {
            base.VisitAttribute(context);

            ArgumentList ac = null;

            if (context.argument_list() != null)
                ac = (ArgumentList) Stack.Pop();

            var result =
                new AttributeSyntax(context.ToLineInfo(), SyntaxKind.Attribute, ac, null);
            Stack.PeekType<AttributeList>().Add(result);

            return result;
        }

        public override SyntaxNode VisitName(ZSharpParser.NameContext context)
        {
            Stack.Push(new IdentifierName(context.ToLineInfo(), SyntaxKind.NameExpression,
                context.IDENTIFIER().Identifier()));
            return null;
        }

        public override SyntaxNode VisitStructureType(ZSharpParser.StructureTypeContext context)
        {
            var result = TypeSyntaxHelper.Create(context.ToLineInfo(), context.GetText());
            Stack.Push(result);
            return result;
        }

        // public override SyntaxNode VisitPrimitiveType(ZSharpParser.PrimitiveTypeContext context)
        // {
        //     TypeNodeKind t = TypeNodeKind.Unknown;
        //
        //     if (context.STRING() != null) t = TypeNodeKind.String;
        //     else if (context.INT() != null) t = TypeNodeKind.Int;
        //     else if (context.UID() != null) t = TypeNodeKind.Uid;
        //     else if (context.OBJECT() != null) t = TypeNodeKind.Object;
        //     else if (context.BOOL() != null) t = TypeNodeKind.Boolean;
        //     else if (context.DOUBLE() != null) t = TypeNodeKind.Double;
        //     else if (context.CHAR() != null) t = TypeNodeKind.Char;
        //     else if (context.VOID() != null) t = TypeNodeKind.Void;
        //
        //     if (t == TypeNodeKind.Unknown)
        //         throw new Exception("Unknown primitive type");
        //     var result = new PrimitiveTypeSyntax(context.ToLineInfo(), t);
        //
        //     Stack.Push(result);
        //
        //     return result;
        // }

        public override SyntaxNode VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = new ArrayTypeSyntax(context.ToLineInfo(), SyntaxKind.ArrayType, Stack.PopType());

            Stack.Push(result);
            return result;
        }

        public override SyntaxNode VisitLiteral(ZSharpParser.LiteralContext context)
        {
            Literal result = null;
            ILineInfo li = context.ToLineInfo();
            if (context.string_literal() != null)
            {
                //Строки парсятся сюда вместе с кавычками и чтобы их убрать приходится
                //заниматься таким вот извращением

                var text = context.string_literal().REGULAR_STRING()?.GetText() ??
                           context.string_literal().VERBATIUM_STRING()?.GetText();

                text = Regex.Unescape(text ?? throw new NullReferenceException());

                if (context.string_literal().REGULAR_STRING() != null)
                    result = new Literal(li, SyntaxKind.LiteralExpression, text.Substring(1, text.Length - 2), false);
                else
                    result = new Literal(li, SyntaxKind.LiteralExpression, text.Substring(2, text.Length - 3), false);

                result.ObjectiveValue = result.Value;
            }
            else if (context.boolean_literal() != null)
            {
                result = new Literal(li, SyntaxKind.LiteralExpression, context.GetText(), false);
                result.ObjectiveValue = bool.Parse(result.Value);
            }
            else if (context.INTEGER_LITERAL() != null)
            {
                result = new Literal(li, SyntaxKind.LiteralExpression, context.GetText(), false);
                result.ObjectiveValue = int.Parse(result.Value);
            }
            else if (context.REAL_LITERAL() != null)
            {
                result = new Literal(li, SyntaxKind.LiteralExpression, context.GetText(), false);
                result.ObjectiveValue = double.Parse(result.Value);
            }
            else if (context.CHARACTER_LITERAL() != null)
            {
                result = new Literal(li, SyntaxKind.LiteralExpression, context.GetText().Substring(1, 1), false);
                result.ObjectiveValue = result.Value[0];
            }

            //TODO: Не обработанным остался HEX INTEGER LITERAL его необходимо доделать

            if (result == null)
                throw new Exception("Unknown literal");

            Stack.Push(result);

            return result;
        }

        public override SyntaxNode VisitIdentifier(ZSharpParser.IdentifierContext context)
        {
            Stack.Push(new IdentifierToken(context.ToLineInfo(), SyntaxKind.IdentifierToken, context.GetText()));
            return Stack.PeekNode();
        }

        public override SyntaxNode VisitLocal_variable_declarator(ZSharpParser.Local_variable_declaratorContext context)
        {
            base.VisitLocal_variable_declarator(context);

            SyntaxNode decl;

            if (context.local_variable_initializer() != null)
                decl = new VariableDeclaratorSyntax(context.ToLineInfo(), SyntaxKind.VariableDeclarator,
                    Stack.PopExpression(), Stack.PopIdentifier());
            else
                decl = new VariableDeclaratorSyntax(context.ToLineInfo(), SyntaxKind.VariableDeclarator,
                    null, Stack.PopIdentifier());

            Stack.Push(decl);

            return Stack.PeekNode();
        }

        public override SyntaxNode VisitLocal_variable_declaration(
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

            Stack.Push(new VariableDeclarationSyntax(context.ToLineInfo(), SyntaxKind.VariableDeclaration,
                Stack.PopType(), list));

            return Stack.PeekNode();
        }

        public override SyntaxNode VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpressionSyntax(context.ToLineInfo(), SyntaxKind.CastExpression,
                Stack.PopExpression(),
                Stack.PopType(), UnaryOperatorType.Cast);

            Stack.Push(result);

            return result;
        }

        public override SyntaxNode VisitMethod_declaration(ZSharpParser.Method_declarationContext context)
        {
            base.VisitMethod_declaration(context);

            MethodDeclarationSyntax result = null;
            ParameterList pc = new ParameterList();
            AttributeList ac = new AttributeList();
            GenericParameterList gpc = new GenericParameterList();

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

            if (context.attributes() != null)
            {
                ac = (AttributeList) Stack.Pop();
            }

            Stack.Push(new MethodDeclarationSyntax(context.ToLineInfo(), SyntaxKind.MethodDeclaration, body, pc, gpc,
                ac,
                funcName, type));

            return Stack.PeekNode();
        }

        public override SyntaxNode VisitParameters(ZSharpParser.ParametersContext context)
        {
            Stack.Push(new ParameterList());
            return base.VisitParameters(context);
        }

        public override SyntaxNode VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = Stack.PeekType<ParameterList>();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new ParameterSyntax(context.ToLineInfo(), SyntaxKind.Parameter,
                Stack.PopType(), context.IDENTIFIER().Identifier(), passMethod);

            paramList.Add(parameter);

            return null;
        }

        public override SyntaxNode VisitBlock(ZSharpParser.BlockContext context)
        {
            PushStack();
            base.VisitBlock(context);
            var sttmts = PopStack();
            Stack.Push(new BlockStatementSyntax(context.ToLineInfo(), SyntaxKind.BlockStatement,
                sttmts.ToCollection<StatementList>()));
            return Stack.PeekNode();
        }

        public override SyntaxNode VisitArgument_list(ZSharpParser.Argument_listContext context)
        {
            Stack.Push(new ArgumentList());

            base.VisitArgument_list(context);

            return null;
        }

        public override SyntaxNode VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);
            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new ArgumentSyntax(context.ToLineInfo(), SyntaxKind.Argument, Stack.PopExpression(),
                passMethod);

            Stack.PeekType<ArgumentList>().Add(result);
            return result;
        }

        public override SyntaxNode VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            return Stack.Peek() as SyntaxNode;
        }

        public override SyntaxNode VisitExpressionPostfix(ZSharpParser.ExpressionPostfixContext context)
        {
            base.VisitExpressionPostfix(context);

            if (context.indexerExpression != null)
                Stack.Push(new IndexerExpressionSyntax(context.ToLineInfo(), SyntaxKind.IndexerExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), UnaryOperatorType.Indexer));

            return null;
        }

        public override SyntaxNode VisitExpression_unary(ZSharpParser.Expression_unaryContext context)
        {
            base.VisitExpression_unary(context);

            var li = context.ToLineInfo();

            if (context.PLUS() != null)
                Stack.Push(new LogicalOrArithmeticExpressionSyntax(li, SyntaxKind.PlusToken,
                    Stack.PopExpression(), UnaryOperatorType.Positive));
            if (context.MINUS() != null)
                Stack.Push(new LogicalOrArithmeticExpressionSyntax(li, SyntaxKind.MinusToken,
                    Stack.PopExpression(), UnaryOperatorType.Negative));
            if (context.BANG() != null)
                Stack.Push(
                    new LogicalOrArithmeticExpressionSyntax(li, SyntaxKind.BangToken, Stack.PopExpression(),
                        UnaryOperatorType.Not));


            return Stack.PeekNode();
        }

        public override SyntaxNode VisitExpression_multiplicative(
            ZSharpParser.Expression_multiplicativeContext context)
        {
            base.VisitExpression_multiplicative(context);

            if (context.expression_multiplicative() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PERCENT() != null) opType = BinaryOperatorType.Modulo;
                if (context.DIV() != null) opType = BinaryOperatorType.Divide;
                if (context.STAR() != null) opType = BinaryOperatorType.Multiply;

                Stack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), opType));
            }

            return null;
        }

        public override SyntaxNode VisitExpression_equality(ZSharpParser.Expression_equalityContext context)
        {
            base.VisitExpression_equality(context);

            if (context.expression_equality() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;


                if (context.OP_EQ() != null) opType = BinaryOperatorType.Equal;
                if (context.OP_NE() != null) opType = BinaryOperatorType.NotEqual;

                Stack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), opType));
            }

            return null;
        }

        public override SyntaxNode VisitSql_literal(ZSharpParser.Sql_literalContext context)
        {
            // var text = context.GetText();
            // text = Regex.Unescape(text ?? throw new NullReferenceException());
            // text = text.Substring(2, text.Length - 3);
            //
            // var result = new Literal(context.ToLineInfo(), text,
            //     new PrimitiveTypeSyntax(null, TypeNodeKind.String), true);
            // Stack.Push(result);
            //
            // return base.VisitSql_literal(context);

            return null;
        }

        public override SyntaxNode VisitExpression_relational(ZSharpParser.Expression_relationalContext context)
        {
            base.VisitExpression_relational(context);

            if (context.expression_relational() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.GT() != null) opType = BinaryOperatorType.GreaterThen;
                if (context.LT() != null) opType = BinaryOperatorType.LessThen;
                if (context.OP_GE() != null) opType = BinaryOperatorType.GraterOrEqualTo;
                if (context.OP_LE() != null) opType = BinaryOperatorType.LessOrEqualTo;

                Stack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), opType));
            }


            return null;
        }

        public override SyntaxNode VisitExpression_additive(ZSharpParser.Expression_additiveContext context)
        {
            base.VisitExpression_additive(context);

            if (context.expression_additive() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PLUS() != null) opType = BinaryOperatorType.Add;
                if (context.MINUS() != null) opType = BinaryOperatorType.Subtract;

                var result = new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), opType);

                Stack.Push(result);
            }


            return null;
        }

        public override SyntaxNode VisitExpression_binary(ZSharpParser.Expression_binaryContext context)
        {
            base.VisitExpression_binary(context);

            if (context.expression_binary() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.OP_AND() != null)
                    opType = BinaryOperatorType.And;
                if (context.OP_OR() != null)
                    opType = BinaryOperatorType.Or;

                if (opType == BinaryOperatorType.None)
                    throw new Exception("this should never happen");

                Stack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    Stack.PopExpression(),
                    Stack.PopExpression(), opType));
            }

            return null;
        }

        public override SyntaxNode VisitAssignment(ZSharpParser.AssignmentContext context)
        {
            base.VisitAssignment(context);

            Stack.Push(new AssignmentSyntax(context.ToLineInfo(), SyntaxKind.EqualsToken,
                Stack.PopExpression(), Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override SyntaxNode VisitThrowStatement(ZSharpParser.ThrowStatementContext context)
        {
            // base.VisitThrowStatement(context);
            //
            // var result = new Throw(context.ToLineInfo(), Stack.PopExpression());
            // Stack.Push(result);
            //
            // return result;

            return null;
        }

        public override SyntaxNode VisitNewExpression(ZSharpParser.NewExpressionContext context)
        {
            // base.VisitNewExpression(context);
            //
            // var ns = context.@namespace()?.GetText();
            //
            // var result = new New(context.ToLineInfo(), ns, Stack.Pop<Call>());
            //
            // Stack.Push(result);
            //
            // return result;

            return null;
        }

        public override SyntaxNode VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            StatementSyntax @else = null;

            if (context.ELSE() != null)
            {
                @else = Stack.PopStatement();
            }

            var result = new If(context.ToLineInfo(), SyntaxKind.IfStatement, @else, Stack.PopStatement(),
                Stack.PopExpression());

            Stack.Push(result);

            return result;
        }

        public override SyntaxNode VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(context.ToLineInfo(), SyntaxKind.ForStatement, Stack.PopInstructionsBody(),
                Stack.PopExpression(),
                Stack.PopExpression(), Stack.PopExpression());

            Stack.Push(result);

            return result;
        }

        public override SyntaxNode VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(context.ToLineInfo(), SyntaxKind.WhileStatement, Stack.PopInstructionsBody(),
                Stack.PopExpression());
            Stack.Push(result);

            return result;
        }

        public override SyntaxNode VisitExpressionStatement(ZSharpParser.ExpressionStatementContext context)
        {
            base.VisitExpressionStatement(context);

            Stack.Push(new ExpressionStatement(context.ToLineInfo(), SyntaxKind.ExpressionStatement,
                Stack.PopExpression()));

            return Stack.PeekNode();
        }

        public override SyntaxNode VisitGlobalVar(ZSharpParser.GlobalVarContext context)
        {
            // base.VisitGlobalVar(context);
            // Stack.Push(new GlobalVar(context.ToLineInfo(), Stack.PopExpression()));
            return null;
        }
    }
}