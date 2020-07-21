using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Aquila.Compiler.Parser;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Misc;
using Portable.Xaml;


namespace Aquila.Compiler
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<SyntaxNode>
    {
        private SyntaxStack _syntaxStack;

        public ZLanguageVisitor()
        {
            _syntaxStack = new SyntaxStack();
        }

        public override SyntaxNode VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            _syntaxStack.Clear();

            var usings = new UsingList();

            foreach (var atd in context.usingSection())
            {
                usings.Add((UsingBase) Visit(atd));
            }

            base.VisitEntryPoint(context);

            var cu = new CompilationUnitSyntax(context.ToLineInfo(), SyntaxKind.CompilationUnit, usings,
                _syntaxStack.Pop<MethodList>(), FieldList.Empty);

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
                _syntaxStack.PopIdentifier(),
                _syntaxStack.PopType());
            _syntaxStack.PeekCollection().Add(f);
            return f;
        }

        public override SyntaxNode VisitPropertyDeclaration(ZSharpParser.PropertyDeclarationContext context)
        {
            // base.VisitPropertyDeclaration(context);
            //
            // BlockSyntax set = null, get = null;
            // if (context.setInst != null)
            // {
            //     set = _syntaxStack.PopInstructionsBody();
            // }
            //
            // if (context.getInst != null)
            // {
            //     get = _syntaxStack.PopInstructionsBody();
            // }
            //
            // Property p =
            //     new PropertyDeclarationSynta(context.ToLineInfo(), _syntaxStack.PopName().Identifier.Value, _syntaxStack.PopType(),
            //         context.GET() != null, context.SET() != null)
            //     {
            //         Getter = get,
            //         Setter = set
            //     };
            //
            // _syntaxStack.PeekCollection().Add(p);
            // return p;

            return null;
        }

        public override SyntaxNode VisitAttributes(ZSharpParser.AttributesContext context)
        {
            _syntaxStack.Push(new AttributeList());
            base.VisitAttributes(context);

            return null;
        }

        public override SyntaxNode VisitAttribute(ZSharpParser.AttributeContext context)
        {
            base.VisitAttribute(context);

            ArgumentList ac = null;

            if (context.arguments() != null)
                ac = (ArgumentList) _syntaxStack.Pop();

            var result =
                new AttributeSyntax(context.ToLineInfo(), SyntaxKind.Attribute, ac, null);
            _syntaxStack.PeekType<AttributeList>().Add(result);

            return result;
        }

        public override SyntaxNode VisitName(ZSharpParser.NameContext context)
        {
            _syntaxStack.Push(new IdentifierName(context.ToLineInfo(), SyntaxKind.NameExpression,
                context.IDENTIFIER().Identifier()));
            return null;
        }

        public override SyntaxNode VisitStructureType(ZSharpParser.StructureTypeContext context)
        {
            var result = TypeSyntaxHelper.Create(context.ToLineInfo(), context.GetText());
            _syntaxStack.Push(result);
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
        //     _syntaxStack.Push(result);
        //
        //     return result;
        // }

        public override SyntaxNode VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = new ArrayTypeSyntax(context.ToLineInfo(), SyntaxKind.ArrayType, _syntaxStack.PopType());

            _syntaxStack.Push(result);
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

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitVariableDeclaration(ZSharpParser.VariableDeclarationContext context)
        {
            base.VisitVariableDeclaration(context);

            SyntaxNode result;
            ExpressionSyntax value;
            TypeSyntax type;

            string varName = context.IDENTIFIER()?.GetText() ?? throw new Exception("Variable name not found");

            if (context.expression() == null)
            {
                value = null;
            }
            else
            {
                value = _syntaxStack.PopExpression();
            }

            type = _syntaxStack.PopType();

            result = new VariableDeclarationSyntax(context.ToLineInfo(), SyntaxKind.VariableDeclaration, value,
                _syntaxStack.PopIdentifier(), type);

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpressionSyntax(context.ToLineInfo(), SyntaxKind.CastExpression,
                _syntaxStack.PopExpression(),
                _syntaxStack.PopType(), UnaryOperatorType.Cast);

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitLookupExpression(ZSharpParser.LookupExpressionContext context)
        {
            // base.VisitLookupExpression(context);
            //
            // SyntaxNode result;
            // if (context.name() != null)
            // {
            //     result = new Mem(context.ToLineInfo(), SyntaxKind.MemberAccessExpression,
            //         _syntaxStack.PopExpression(),
            //         _syntaxStack.PopExpression());
            // }
            // else
            // {
            //     result = new MethodLookupExpression(context.ToLineInfo(), _syntaxStack.PopExpression(),
            //         _syntaxStack.PopExpression());
            // }
            //
            // _syntaxStack.Push(result);
            //
            // return result;

            return null;
        }


        public override SyntaxNode VisitMethodDeclaration(ZSharpParser.MethodDeclarationContext context)
        {
            base.VisitMethodDeclaration(context);

            MethodDeclarationSyntax result = null;
            ParameterList pc = new ParameterList();
            AttributeList ac = new AttributeList();
            GenericParameterList gpc = new GenericParameterList();

            var body = _syntaxStack.PopInstructionsBody();

            if (context.genericParameters() != null)
            {
                gpc = (GenericParameterList) _syntaxStack.Pop();
            }

            if (context.parameters() != null)
            {
                pc = (ParameterList) _syntaxStack.Pop();
            }

            var type = _syntaxStack.PopType();

            var funcName = context.IDENTIFIER().Identifier();

            if (context.attributes() != null)
            {
                ac = (AttributeList) _syntaxStack.Pop();
            }

            result = new MethodDeclarationSyntax(context.ToLineInfo(), SyntaxKind.MethodDeclaration, body, pc, gpc, ac,
                funcName, type);
            try
            {
                _syntaxStack.PeekCollection().Add(result);
            }
            catch (InvalidOperationException ioe)
            {
                if (ioe.Message == "Stack empty.")
                {
                    //ignore
                }
                else
                    throw;
            }

            return result;
        }

        public override SyntaxNode VisitInstructionsBody(ZSharpParser.InstructionsBodyContext context)
        {
            base.VisitInstructionsBody(context);
            var sc = (StatementList) _syntaxStack.Pop();
            _syntaxStack.Push(new BlockSyntax(context.ToLineInfo(), SyntaxKind.BlockStatement, sc));
            return null;
        }

        public override SyntaxNode VisitParameters(ZSharpParser.ParametersContext context)
        {
            _syntaxStack.Push(new ParameterList());
            return base.VisitParameters(context);
        }

        public override SyntaxNode VisitGenericParameters(ZSharpParser.GenericParametersContext context)
        {
            _syntaxStack.Push(new GenericParameterList());
            return base.VisitGenericParameters(context);
        }

        public override SyntaxNode VisitGenericParameter(ZSharpParser.GenericParameterContext context)
        {
            // var genericParameterList = _syntaxStack.PeekType<ParameterList>();
            // base.VisitGenericParameter(context);
            //
            // var parameter = new GenericParameter(context.ToLineInfo(), context.IDENTIFIER().GetText());
            //
            // genericParameterList.Add(parameter);

            return null;
        }

        public override SyntaxNode VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = _syntaxStack.PeekType<ParameterList>();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new ParameterSyntax(context.ToLineInfo(), SyntaxKind.Parameter,
                _syntaxStack.PopType(), context.IDENTIFIER().Identifier(), passMethod);

            paramList.Add(parameter);

            return null;
        }


        public override SyntaxNode VisitArguments(ZSharpParser.ArgumentsContext context)
        {
            _syntaxStack.Push(new ArgumentList());

            base.VisitArguments(context);

            return null;
        }

        public override SyntaxNode VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);
            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new ArgumentSyntax(context.ToLineInfo(), SyntaxKind.Argument, _syntaxStack.PopExpression(),
                passMethod);

            _syntaxStack.PeekType<ArgumentList>().Add(result);
            return result;
        }

        public override SyntaxNode VisitFunctionCall(ZSharpParser.FunctionCallContext context)
        {
            // base.VisitFunctionCall(context);
            //
            // ArgumentList args = new ArgumentList();
            //
            // if (context.arguments() != null)
            // {
            //     args = _syntaxStack.Pop<ArgumentList>();
            // }
            //
            // var result = new Call(context.ToLineInfo(), args, _syntaxStack.PopName(), null);
            //
            // _syntaxStack.Push(result);
            //
            // return result;

            return null;
        }

        public override SyntaxNode VisitStatements(ZSharpParser.StatementsContext context)
        {
            _syntaxStack.Push(new StatementList());

            base.VisitStatements(context);
            return null;
        }

        public override SyntaxNode VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            return null;
        }

        public override SyntaxNode VisitExpressionPostfix(ZSharpParser.ExpressionPostfixContext context)
        {
            base.VisitExpressionPostfix(context);

            if (context.indexerExpression != null)
                _syntaxStack.Push(new IndexerExpressionSyntax(context.ToLineInfo(), SyntaxKind.IndexerExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), UnaryOperatorType.Indexer));

            return null;
        }

        public override SyntaxNode VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
        {
            // base.VisitExpressionUnary(context);
            //
            // var li = context.ToLineInfo();
            //
            // if (context.PLUS() != null)
            //     _syntaxStack.Push(new UnaryEx(li, SyntaxKind.BinaryExpression, _syntaxStack.PopExpression(),));
            // if (context.MINUS() != null)
            //     _syntaxStack.Push(new LogicalOrArithmeticExpression(li, _syntaxStack.PopExpression(),
            //         UnaryOperatorType.Negative));
            // if (context.BANG() != null)
            //     _syntaxStack.Push(
            //         new LogicalOrArithmeticExpression(li, _syntaxStack.PopExpression(), UnaryOperatorType.Not));
            //

            return null;
        }

        public override SyntaxNode VisitExpressionMultiplicative(
            ZSharpParser.ExpressionMultiplicativeContext context)
        {
            base.VisitExpressionMultiplicative(context);

            if (context.expressionMultiplicative() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PERCENT() != null) opType = BinaryOperatorType.Modulo;
                if (context.DIV() != null) opType = BinaryOperatorType.Divide;
                if (context.STAR() != null) opType = BinaryOperatorType.Multiply;

                _syntaxStack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
            }

            return null;
        }

//        public override SyntaxNode VisitExtensionExpression(ZSharpParser.ExtensionExpressionContext context)
//        {
//            base.VisitExtensionExpression(context);
//            Extension result;
//
//            var extensionObj = _syntaxStack.Pop();
//            if (extensionObj is Block ib)
//            {
//                result = new Extension(context.ToLineInfo(), _syntaxStack.PopString(),
//                    ExtensionKind.Instructions);
//                result.Block = ib;
//            }
//            else
//            {
//                var path = _syntaxStack.PopString();
//                var extensionName = path.Split('.')[0];
//                result = new Extension(context.ToLineInfo(), extensionName);
//                result.Path = path;
//            }
//
//            // Вот тут мы должны сделать следующее: 
//            // 1) Установить, какой хендлер это обрабатывает
//            // 2) Вызвать обработчик. Он должен переопределить дерево вызовов.
//
//            //_syntaxStack.Push();
//
//
//            if (!ExtensionManager.Managers.TryGetValue(result.ExtensionName, out var ext))
//            {
//                throw new Exception($"The extension {result.ExtensionName} not found or not loaded");
//            }
//
//            //TODO: Закончить разработку расширений
//            //_syntaxStack.Push();
//            return null;
//        }

        public override SyntaxNode VisitExpressionEquality(ZSharpParser.ExpressionEqualityContext context)
        {
            base.VisitExpressionEquality(context);

            if (context.expressionEquality() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;


                if (context.OP_EQ() != null) opType = BinaryOperatorType.Equal;
                if (context.OP_NE() != null) opType = BinaryOperatorType.NotEqual;

                _syntaxStack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
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
            // _syntaxStack.Push(result);
            //
            // return base.VisitSql_literal(context);

            return null;
        }

        public override SyntaxNode VisitExpressionRelational(ZSharpParser.ExpressionRelationalContext context)
        {
            base.VisitExpressionRelational(context);

            if (context.expressionRelational() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.GT() != null) opType = BinaryOperatorType.GreaterThen;
                if (context.LT() != null) opType = BinaryOperatorType.LessThen;
                if (context.OP_GE() != null) opType = BinaryOperatorType.GraterOrEqualTo;
                if (context.OP_LE() != null) opType = BinaryOperatorType.LessOrEqualTo;

                _syntaxStack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
            }


            return null;
        }

        public override SyntaxNode VisitExpressionAdditive(ZSharpParser.ExpressionAdditiveContext context)
        {
            base.VisitExpressionAdditive(context);

            if (context.expressionAdditive() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PLUS() != null) opType = BinaryOperatorType.Add;
                if (context.MINUS() != null) opType = BinaryOperatorType.Subtract;

                var result = new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType);

                _syntaxStack.Push(result);
            }


            return null;
        }

        public override SyntaxNode VisitExpressionBinary(ZSharpParser.ExpressionBinaryContext context)
        {
            base.VisitExpressionBinary(context);

            if (context.expressionBinary() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.OP_AND() != null)
                    opType = BinaryOperatorType.And;
                if (context.OP_OR() != null)
                    opType = BinaryOperatorType.Or;

                if (opType == BinaryOperatorType.None)
                    throw new Exception("this should never happen");

                _syntaxStack.Push(new BinaryExpressionSyntax(context.ToLineInfo(), SyntaxKind.BinaryExpression,
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
            }

            return null;
        }

        public override SyntaxNode VisitAssigment(ZSharpParser.AssigmentContext context)
        {
            base.VisitAssigment(context);

            // Expression result;
            //
            // if (context.indexExpression != null)
            //     result = new Assignment(context.ToLineInfo(), _syntaxStack.PopExpression(),
            //         _syntaxStack.PopExpression(), (ICanBeAssigned) _syntaxStack.PopExpression());
            // else if (context.OP_INC() != null)
            // {
            //     result = new PostIncrementExpression(context.ToLineInfo(), _syntaxStack.PopExpression());
            // }
            // else if (context.OP_DEC() != null)
            // {
            //     result = new PostDecrementExpression(context.ToLineInfo(), _syntaxStack.PopExpression());
            // }
            // else
            //     result = new Assignment(context.ToLineInfo(), _syntaxStack.PopExpression(), null,
            //         (ICanBeAssigned) _syntaxStack.PopExpression());
            //
            // _syntaxStack.Push(result);
            //
            // return result;

            return null;
        }


        public override SyntaxNode VisitThrowStatement(ZSharpParser.ThrowStatementContext context)
        {
            // base.VisitThrowStatement(context);
            //
            // var result = new Throw(context.ToLineInfo(), _syntaxStack.PopExpression());
            // _syntaxStack.Push(result);
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
            // var result = new New(context.ToLineInfo(), ns, _syntaxStack.Pop<Call>());
            //
            // _syntaxStack.Push(result);
            //
            // return result;

            return null;
        }


        public override SyntaxNode VisitStatement(ZSharpParser.StatementContext context)
        {
            base.VisitStatement(context);

            SyntaxNode result = null;

            if (context.RETURN() != null)
            {
                if (context.returnExpression == null)
                    result = new ReturnSyntax(context.ToLineInfo(), SyntaxKind.ReturnStatement, null);
                else
                    result = new ReturnSyntax(context.ToLineInfo(), SyntaxKind.ReturnStatement,
                        _syntaxStack.PopExpression());

                _syntaxStack.PeekType<StatementList>().Add(result);
            }
            else
            {
                SyntaxNode node = (SyntaxNode) _syntaxStack.Pop();

                // По умолчанию все операции могут являться выражениями.
                //перед тем как мы будем добавлять их в инструкции нужно обернуть их в инструкцию
                if (node is ExpressionSyntax exp)
                {
                    node = new ExpressionStatement(context.ToLineInfo(), SyntaxKind.ExpressionStatement, exp);
                }

                _syntaxStack.PeekType<StatementList>().Add(node);
            }

            return result;
        }

        public override SyntaxNode VisitInstructionsOrSingleStatement(
            ZSharpParser.InstructionsOrSingleStatementContext context)
        {
            var sc = new StatementList();

            if (context.statement() != null)
                _syntaxStack.Push(sc);

            base.VisitInstructionsOrSingleStatement(context);

            if (context.statement() != null)
            {
                _syntaxStack.Pop();
                _syntaxStack.Push(new BlockSyntax(context.statement().ToLineInfo(), SyntaxKind.BlockStatement, sc));
            }

            return null;
        }

        public override SyntaxNode VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            BlockSyntax @else = null;

            if (context.ELSE() != null)
            {
                @else = _syntaxStack.PopInstructionsBody();
            }

            var result = new If(context.ToLineInfo(), SyntaxKind.IfStatement, @else, _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(context.ToLineInfo(), SyntaxKind.ForStatement, _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression(),
                _syntaxStack.PopExpression(), _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(context.ToLineInfo(), SyntaxKind.WhileStatement, _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());
            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitTryStatement(ZSharpParser.TryStatementContext context)
        {
            // base.VisitTryStatement(context);
            //
            // Block @catch = null, @finally = null;
            //
            // if (context.finallyExp != null)
            // {
            //     @finally = _syntaxStack.PopInstructionsBody();
            // }
            //
            // if (context.catchExp != null)
            // {
            //     @catch = _syntaxStack.PopInstructionsBody();
            // }
            //
            // var tryNode = new Try(context.ToLineInfo(), _syntaxStack.PopInstructionsBody(), @catch, @finally);
            //
            // _syntaxStack.Push(tryNode);
            //
            // return tryNode;

            return null;
        }


        public override SyntaxNode VisitGlobalVar(ZSharpParser.GlobalVarContext context)
        {
            // base.VisitGlobalVar(context);
            // _syntaxStack.Push(new GlobalVar(context.ToLineInfo(), _syntaxStack.PopExpression()));
            return null;
        }
    }
}