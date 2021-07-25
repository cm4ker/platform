using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Resources;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Aquila.CodeAnalysis.Syntax.Parser;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Expressions;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Ast.Statements;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;
using MoreLinq.Extensions;

namespace Aquila.Syntax.Parser
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<LangElement>
    {
        private readonly string _code;
        private readonly string _filePath;
        private Stack<SyntaxStack> _stackOfStacks;
        private bool _isGlobal;

        private SyntaxStack Stack => _stackOfStacks.Peek();

        public ZLanguageVisitor(string code, string filePath)
        {
            _code = code;
            _filePath = filePath;
            _stackOfStacks = new Stack<SyntaxStack>();
            PushStack();
        }


        public override LangElement Visit(IParseTree tree)
        {
            return VisitCore(tree);
        }

        private LangElement VisitCore(IParseTree tree)
        {
            var ast = base.Visit(tree);

            if (ast != null)
                ast.UpdateTree();

            return ast;
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
            PushStack();
            if (context.import_directives() != null)
                base.VisitImport_directives(context.import_directives());
            var imports = (context.import_directives()?.import_directive().Length > 0)
                ? PopStack().ToCollection<ImportList, ImportBase>()
                : ImportList.Empty;


            PushStack();
            //resolve all methods
            context.global_method_declaration().ForEach(x => VisitGlobal_method_declaration(x));
            var methods = (context.global_method_declaration().Length > 0)
                ? PopStack().ToCollection<MethodList, MethodDecl>()
                : MethodList.Empty;

            PushStack();
            //resolve all components decl
            context.component_declaration().ForEach(x => VisitComponent_declaration(x));
            var namespaces = PopStack().ToCollection<ComponentList, ComponentDecl>();

            var cu = new SourceUnit(context.ToLineInfo(), SyntaxKind.CompilationUnit, _code,
                _filePath,
                imports,
                methods,
                ExtendList.Empty,
                namespaces);

            return cu;
        }

        public override LangElement VisitImport_directives(ZSharpParser.Import_directivesContext context)
        {
            PushStack();
            base.VisitImport_directives(context);
            var col = PopStack().ToCollection<ImportList, ImportBase>();
            Stack.Push(col);

            return Stack.PeekNode();
        }

        public override LangElement VisitUsingNamespaceDirective(ZSharpParser.UsingNamespaceDirectiveContext context)
        {
            Stack.Push(new ImportDecl(context.ToLineInfo(), SyntaxKind.ImportDerictive,
                context.namespace_or_type_name().GetText()));

            return Stack.PeekNode();
        }

        public override LangElement VisitComponent_declaration(ZSharpParser.Component_declarationContext context)
        {
            PushStack();
            //resolve all extends
            var extendContext = context.component_body()?.component_member_declarations()
                ?.component_member_declaration();
            extendContext?.ForEach(x => VisitComponent_member_declaration(x));
            var extends = (extendContext?.Length > 0)
                ? PopStack().ToCollection<ExtendList, ExtendDecl>()
                : ExtendList.Empty;

            Visit(context.qualified_identifier());

            Stack.Push(new ComponentDecl(context.ToLineInfo(), SyntaxKind.NamespaceDeclaration, extends,
                (QualifiedIdentifierToken)Stack.Pop()));

            return Stack.PeekNode();
        }

        public override LangElement VisitType_extend(ZSharpParser.Type_extendContext context)
        {
            PushStack();
            //resolve all methods
            //TODO: Make all members based from MemberDecl
            // var methodsContext = context.extend_body()?.extend_member_declarations()?.extend_member_declaration()
            //     .Select(x => x.common_member_declaration())
            //     .Select(x => x.method_declaration()).ToArray();
            //
            // methodsContext?.ForEach(x => VisitMethod_declaration(x));
            VisitExtend_member_declarations(context.extend_body().extend_member_declarations());

            var methods = PopStack().ToCollection<MethodList, MethodDecl>();

            VisitIdentifier(context.identifier());

            Stack.Push(new ExtendDecl(context.ToLineInfo(), SyntaxKind.TypeExtendDeclaration, methods,
                Stack.PopIdentifier()));

            return Stack.PeekNode();
        }

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

        public override LangElement VisitLocal_variable_type(ZSharpParser.Local_variable_typeContext context)
        {
            if (context.VAR() != null)
            {
                Stack.Push(TypeSyntaxHelper.Create(context.ToLineInfo(), context.GetText()));
                return Stack.PeekNode();
            }
            else
                return base.VisitLocal_variable_type(context);
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
                result = new LiteralEx(li, SyntaxKind.LiteralExpression, Operations.IntLiteral, context.GetText(),
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

        public override LangElement VisitQualified_identifier(ZSharpParser.Qualified_identifierContext context)
        {
            Stack.Push(new QualifiedIdentifierToken(context.ToLineInfo(), SyntaxKind.QualifiedIdentifierToken,
                context.GetText()));
            return Stack.PeekNode();
        }

        public override LangElement VisitLocal_variable_declarator(
            ZSharpParser.Local_variable_declaratorContext context)
        {
            base.VisitLocal_variable_declarator(context);

            LangElement decl;
            Expression initializer = null;

            if (context.local_variable_initializer() != null)
                initializer = Stack.PopExpression();
            else
            {
                if (context.exception is InputMismatchException)
                    initializer = new MissingEx(context.exception.OffendingToken.ToLineInfo(), SyntaxKind.BadToken,
                        Operations.Empty, context.exception.Message);
            }

            decl = new VarDeclarator(context.ToLineInfo(), SyntaxKind.VariableDeclarator, initializer,
                Stack.PopIdentifier());

            Stack.Push(decl);

            return Stack.PeekNode();
        }

        public override LangElement VisitDeclarationStatement(ZSharpParser.DeclarationStatementContext context)
        {
            return VisitLocal_variable_declaration(context.local_variable_declaration());
        }

        public override LangElement VisitLocal_variable_declaration(
            ZSharpParser.Local_variable_declarationContext context)
        {
            VisitLocal_variable_type(context.local_variable_type());

            DeclaratorList list;

            {
                PushStack();
                foreach (var declarator in context.local_variable_declarator())
                {
                    VisitLocal_variable_declarator(declarator);
                }

                var declarators = PopStack();
                list = declarators.ToCollection<DeclaratorList, VarDeclarator>();
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

        public override LangElement VisitGlobal_method_declaration(
            ZSharpParser.Global_method_declarationContext context)
        {
            VisitAll_member_modifiers(context.all_member_modifiers());
            _isGlobal = true;
            VisitMethod_declaration(context.method_declaration());
            _isGlobal = false;
            return Stack.PeekNode();
        }

        public override LangElement VisitAll_member_modifiers(ZSharpParser.All_member_modifiersContext context)
        {
            PushStack();

            void PushToken(ParserRuleContext context, ITerminalNode token)
            {
                if (token != null)
                    Stack.Push(new ModifierToken(context.ToLineInfo(), SyntaxKind.ModifierToken, token.GetText()));
            }

            if (context != null)
                foreach (var modifier in context.all_member_modifier())
                {
                    PushToken(modifier, modifier.NEW());
                    PushToken(modifier, modifier.STATIC());
                    PushToken(modifier, modifier.PUBLIC());
                }

            var list = PopStack().ToCollection<ModifierList, ModifierToken>();
            Stack.Push(list);

            return Stack.PeekNode();
        }

        public override LangElement VisitMethod_declaration(ZSharpParser.Method_declarationContext context)
        {
            MethodDecl result = null;
            Visit(context.return_type());
            Visit(context.method_body());
            PushStack();
            if (context.parameters() != null)
                VisitParameters(context.parameters());

            var p = PopStack();
            ParameterList pc = p.ToCollection<ParameterList, Parameter>();
            AnnotationList ac = new AnnotationList(ImmutableArray<Annotation>.Empty);

            var body = Stack.PopInstructionsBody();

            var type = Stack.PopType();

            var funcName = context.IDENTIFIER().Identifier();

            Stack.Push(new MethodDecl(context.ToLineInfo(), SyntaxKind.MethodDeclaration, body, pc, //gpc,
                ac,
                funcName, type, Stack.Pop<ModifierList>(), _isGlobal));

            return Stack.PeekNode();
        }

        public override LangElement VisitParameter(ZSharpParser.ParameterContext context)
        {
            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new Parameter(context.ToLineInfo(), SyntaxKind.Parameter,
                Stack.PopType(), context.IDENTIFIER().Identifier(), passMethod);

            Stack.Push(parameter);

            return Stack.PeekNode();
        }

        public override LangElement VisitBlock(ZSharpParser.BlockContext context)
        {
            PushStack();
            base.VisitBlock(context);
            var sttmts = PopStack();
            Stack.Push(new BlockStmt(context.ToLineInfo(), SyntaxKind.BlockStatement,
                sttmts.ToCollection<StatementList, Statement>()));
            return Stack.PeekNode();
        }

        // public override LangElement VisitSimple_embedded_statement(
        //     ZSharpParser.Simple_embedded_statementContext context)
        // {
        //     base.VisitSimple_embedded_statement(context);
        //
        //     //then we meet this construction:
        //     //if (cond)
        //     //  then_statement
        //     //
        //     //then we create virtual block for this (not decide right or not)
        //     //
        //     // result is
        //     // if (cond)
        //     // {
        //     //    then_statement
        //     // }. 
        //
        //     var block = new BlockStmt(context.ToLineInfo(), SyntaxKind.BlockStatement,
        //         new StatementList(new ImmutableArray<Statement> {Stack.PopStatement()}));
        //     Stack.Push(block);
        //
        //     return Stack.PeekNode();
        // }

        public override LangElement VisitArgument_list(ZSharpParser.Argument_listContext context)
        {
            PushStack();
            base.VisitArgument_list(context);
            var args = PopStack().ToCollection<ArgumentList, Argument>();
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

        public override LangElement VisitStatement(ZSharpParser.StatementContext context)
        {
            return base.VisitStatement(context);
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
            if (context.OP_INC() != null)
                Stack.Push(new IncDecEx(li, SyntaxKind.IncDecExpression, Operations.IncDec, Stack.PopExpression(), true,
                    false));
            if (context.OP_DEC() != null)
                Stack.Push(new IncDecEx(li, SyntaxKind.IncDecExpression, Operations.IncDec, Stack.PopExpression(),
                    false, false));


            return Stack.PeekNode();
        }


        private void CreateBinaryOp(ParserRuleContext currentContext, ParserRuleContext[] childContexts,
            Func<int, Operations> factory)
        {
            var length = childContexts.Length;

            if (length > 1)
            {
                for (int i = 1; i < length; i++)
                {
                    var indexChild = i * 2;

                    var sign = currentContext.GetChild(indexChild - 1) as TerminalNodeImpl ??
                               throw new Exception("Unexpected error");

                    Operations opType = factory(sign.Symbol.Type);

                    var right = Stack.PopExpression();
                    var left = Stack.PopExpression();

                    Stack.Push(new BinaryEx(currentContext.ToLineInfo(), SyntaxKind.BinaryExpression,
                        opType, right, left));
                }
            }
        }

        public override LangElement VisitMultiplicative_expression(
            ZSharpParser.Multiplicative_expressionContext context)
        {
            base.VisitMultiplicative_expression(context);

            CreateBinaryOp(context, context.range_expression(), i => (i) switch
            {
                ZSharpLexer.PERCENT => Operations.Mod,
                ZSharpLexer.DIV => Operations.Div,
                ZSharpLexer.STAR => Operations.Mul,
                _ => throw new InvalidOperationException("Unknown operator")
            });
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

            // if (context.shift_expression().Length > 1)
            // {
            //     Operations opType = Operations.Unknown;
            //
            //     if (context.GT() != null) opType = Operations.GreaterThan;
            //     if (context.LT() != null) opType = Operations.LessThan;
            //     if (context.OP_GE() != null) opType = Operations.GreaterThanOrEqual;
            //     if (context.OP_LE() != null) opType = Operations.LessThanOrEqual;
            //
            //     Stack.Push(new BinaryEx(context.ToLineInfo(), SyntaxKind.BinaryExpression, opType,
            //         Stack.PopExpression(),
            //         Stack.PopExpression()));
            // }

            CreateBinaryOp(context, context.shift_expression(), i => (i) switch
            {
                ZSharpLexer.GT => Operations.GreaterThan,
                ZSharpLexer.LT => Operations.LessThan,
                ZSharpLexer.OP_GE => Operations.GreaterThanOrEqual,
                ZSharpLexer.OP_LE => Operations.LessThanOrEqual,
                _ => throw new InvalidOperationException("Unknown operator")
            });

            return null;
        }

        public override LangElement VisitAdditive_expression(ZSharpParser.Additive_expressionContext context)
        {
            base.VisitAdditive_expression(context);

            CreateBinaryOp(context, context.multiplicative_expression(), i => (i) switch
            {
                ZSharpLexer.PLUS => Operations.Add,
                ZSharpLexer.MINUS => Operations.Sub,
                _ => throw new InvalidOperationException("Unknown operator")
            });

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

        public override LangElement VisitPrimary_expression(ZSharpParser.Primary_expressionContext context)
        {
            base.VisitPrimary_expression(context);

            if (context.OP_INC() != null && context.OP_INC().Any())
            {
                Stack.Push(new IncDecEx(context.ToLineInfo(), SyntaxKind.IncDecExpression, Operations.IncDec,
                    Stack.PopExpression(), true, true));
            }

            if (context.OP_DEC() != null && context.OP_DEC().Any())
            {
                Stack.Push(new IncDecEx(context.ToLineInfo(), SyntaxKind.IncDecExpression, Operations.IncDec,
                    Stack.PopExpression(), false, true));
            }

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
                Stack.PopExpression(), Stack.Pop<VarDecl>());

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