using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Extension;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Sre;

namespace ZenPlatform.Compiler.AST
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<object>
    {
        private SyntaxStack _syntaxStack;
        private ITypeSystem _ts;
        private SystemTypeBindings _sb;

        public ZLanguageVisitor(ITypeSystem typeSystem)
        {
            _syntaxStack = new SyntaxStack();
            _ts = typeSystem;
            _sb = new SystemTypeBindings(_ts);
        }

        public override object VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            _syntaxStack.Clear();

            var cu = new CompilationUnit(context.start.ToLineInfo());
            _syntaxStack.Push(cu);


            base.VisitEntryPoint(context);

            return cu;
        }

        public override object VisitUsingDefinition(ZSharpParser.UsingDefinitionContext context)
        {
            base.VisitUsingDefinition(context);
            _syntaxStack.PeekType<CompilationUnit>().Namespaces.Add(_syntaxStack.PopString());

            return null;
        }

        public override object VisitModuleDefinition(ZSharpParser.ModuleDefinitionContext context)
        {
            base.VisitModuleDefinition(context);

            Module result = new Module(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.IDENTIFIER().GetText());

            _syntaxStack.PeekType<CompilationUnit>().TypeEntities.Add(result);

            return result;
        }

        public override object VisitTypeDefinition(ZSharpParser.TypeDefinitionContext context)
        {
            base.VisitTypeDefinition(context);

            var result = new Class(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.IDENTIFIER().GetText());

            _syntaxStack.PeekType<CompilationUnit>().TypeEntities.Add(result);

            return result;
        }


        public override object VisitModuleBody(ZSharpParser.ModuleBodyContext context)
        {
            _syntaxStack.Push(new MemberCollection());
            base.VisitModuleBody(context);

            TypeBody result;

            if (context.ChildCount == 0)
                result = new TypeBody(null);
            else
                result = new TypeBody((MemberCollection) _syntaxStack.Pop());

            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitName(ZSharpParser.NameContext context)
        {
            _syntaxStack.Push(context.GetText());

            return null;
        }

        public override object VisitStructureType(ZSharpParser.StructureTypeContext context)
        {
            var result = new TypeNode(context.start.ToLineInfo(), context.GetText());

            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitPrimitiveType(ZSharpParser.PrimitiveTypeContext context)
        {
            IType result = null;
            if (context.STRING() != null) result = _sb.String;
            else if (context.INT() != null) result = _sb.Int;
            else if (context.BOOL() != null) result = _sb.Bool;
            else if (context.DOUBLE() != null) result = _sb.Double;
            else if (context.CHAR() != null) result = _sb.Char;
            else if (context.VOID() != null) result = _sb.Void;

            if (result == null)
                throw new Exception("Unknown primitive type");
            _syntaxStack.Push(new TypeNode(context.start.ToLineInfo(), result));

            return result;
        }

        public override object VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = new TypeNode(context.start.ToLineInfo(),
                new UnknownArrayType(context.GetText(), _syntaxStack.PopType().Type));

            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitLiteral(ZSharpParser.LiteralContext context)
        {
            AstNode result = null;
            ILineInfo li = context.start.ToLineInfo();
            if (context.string_literal() != null)
            {
                //Строки парсятся сюда вместе с кавычками и чтобы их убрать приходится
                //заниматься таким вот извращением

                var text = context.string_literal().REGULAR_STRING()?.GetText() ??
                           context.string_literal().VERBATIUM_STRING()?.GetText();

                text = Regex.Unescape(text ?? throw new NullReferenceException());

                if (context.string_literal().REGULAR_STRING() != null)
                    result = new Literal(li, text.Substring(1, text.Length - 2), new TypeNode(li, _sb.String));
                else
                    result = new Literal(li, text.Substring(2, text.Length - 3), new TypeNode(li, _sb.String));
            }
            else if (context.boolean_literal() != null)
                result = new Literal(li, context.GetText(), new TypeNode(li, _sb.Bool));
            else if (context.INTEGER_LITERAL() != null)
                result = new Literal(li, context.GetText(), new TypeNode(li, _sb.Int));
            else if (context.REAL_LITERAL() != null)
                result = new Literal(li, context.GetText(), new TypeNode(li, _sb.Double));
            else if (context.CHARACTER_LITERAL() != null)
                result = new Literal(li, context.GetText().Substring(1, 1), new TypeNode(li, _sb.Char));

            //TODO: Не обработанным остался HEX INTEGER LITERAL его необходимо доделать

            if (result == null)
                throw new Exception("Unknown literal");

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitVariableDeclaration(ZSharpParser.VariableDeclarationContext context)
        {
            base.VisitVariableDeclaration(context);

            AstNode result;
            if (context.expression() == null)
                result = new Variable(context.start.ToLineInfo(), null, context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());
            else
                result = new Variable(context.start.ToLineInfo(), _syntaxStack.Pop(), context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());

            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                _syntaxStack.PopType());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitFunctionDeclaration(ZSharpParser.FunctionDeclarationContext context)
        {
            base.VisitFunctionDeclaration(context);
            AstNode result = null;
            ParameterCollection pc = null;

            var body = _syntaxStack.PopInstructionsBody();

            if (context.parameters() != null)
            {
                pc = (ParameterCollection) _syntaxStack.Pop();
            }

            var type = _syntaxStack.PopType();
            var funcName = context.IDENTIFIER().GetText();

            result = new Function(context.start.ToLineInfo(), body, pc, funcName, type);

            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override object VisitInstructionsBody(ZSharpParser.InstructionsBodyContext context)
        {
            base.VisitInstructionsBody(context);
            var sc = (StatementCollection) _syntaxStack.Pop();
            _syntaxStack.Push(new InstructionsBodyNode(sc));
            return null;
        }

        public override object VisitParameters(ZSharpParser.ParametersContext context)
        {
            _syntaxStack.Push(new ParameterCollection());
            return base.VisitParameters(context);
        }


        public override object VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = _syntaxStack.PeekCollection();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new Parameter(context.start.ToLineInfo(), context.IDENTIFIER().GetText(),
                _syntaxStack.PopType(), passMethod);

            paramList.Add(parameter);

            return null;
        }


        public override object VisitArguments(ZSharpParser.ArgumentsContext context)
        {
            _syntaxStack.Push(new ArgumentCollection());

            base.VisitArguments(context);

            return null;
        }

        public override object VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);
            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new Argument(context.start.ToLineInfo(), _syntaxStack.PopExpression(), passMethod);

            _syntaxStack.PeekCollection().Add(result);
            return result;
        }

        public override object VisitFunctionCall(ZSharpParser.FunctionCallContext context)
        {
            base.VisitFunctionCall(context);

            var result = new CallStatement(context.start.ToLineInfo(), (ArgumentCollection) _syntaxStack.Pop(),
                _syntaxStack.PopString());


            _syntaxStack.Push(result);

            return result;
        }


        public override object VisitFunctionCallExpression(ZSharpParser.FunctionCallExpressionContext context)
        {
            base.VisitFunctionCallExpression(context);

            var callStatement = (CallStatement) _syntaxStack.Pop();

            var result = new Call(context.start.ToLineInfo(), callStatement.Arguments, callStatement.Name);

            _syntaxStack.Push(result);
            return null;
        }

        public override object VisitStatements(ZSharpParser.StatementsContext context)
        {
            _syntaxStack.Push(new StatementCollection());
            base.VisitStatements(context);
            return null;
        }

        public override object VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            if (context.expression() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PLUS() != null) opType = BinaryOperatorType.Add;
                if (context.MINUS() != null) opType = BinaryOperatorType.Subtract;

                var result = new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType);

                _syntaxStack.Push(result);
            }

            return null;
        }


        public override object VisitExpressionPrimary(ZSharpParser.ExpressionPrimaryContext context)
        {
            base.VisitExpressionPrimary(context);

            if (context.name() != null)
            {
                var identifier = _syntaxStack.PopString().Split('.');
                var li = context.name().start.ToLineInfo();
                //Если мы пытаемся получить какое-то свойство у переменной, то мы обязательно должны пометить это как PropertyExpression
                // Похожая механика реализована и в FunctionCall
                if (identifier.Length > 1)
                {
                    _syntaxStack.Push(new Name(li, identifier[0]));

                    foreach (var str in identifier.ToList().GetRange(1, identifier.Length - 1))
                    {
                        _syntaxStack.Push(new FieldExpression(_syntaxStack.PopExpression(), str));
                    }
                }
                else
                {
                    _syntaxStack.Push(new Name(li, identifier[0]));
                }
            }

            return null;
        }

        public override object VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
        {
            base.VisitExpressionUnary(context);

            var li = context.start.ToLineInfo();

            if (context.PLUS() != null)
                _syntaxStack.Push(new LogicalOrArithmeticExpression(li, _syntaxStack.PopExpression(),
                    UnaryOperatorType.Positive));
            if (context.MINUS() != null)
                _syntaxStack.Push(new LogicalOrArithmeticExpression(li, _syntaxStack.PopExpression(),
                    UnaryOperatorType.Negative));
            if (context.BANG() != null)
                _syntaxStack.Push(
                    new LogicalOrArithmeticExpression(li, _syntaxStack.PopExpression(), UnaryOperatorType.Not));
            if (context.indexerExpression != null)
                _syntaxStack.Push(new IndexerExpression(li, _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression()));

            return null;
        }

        public override object VisitExpressionTerm(ZSharpParser.ExpressionTermContext context)
        {
            base.VisitExpressionTerm(context);

            if (context.expressionTerm() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.DIV() != null) opType = BinaryOperatorType.Divide;
                if (context.STAR() != null) opType = BinaryOperatorType.Multiply;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
            }

            return null;
        }

        public override object VisitExtensionExpression(ZSharpParser.ExtensionExpressionContext context)
        {
            base.VisitExtensionExpression(context);
            Extension result;

            var extensionObj = _syntaxStack.Pop();
            if (extensionObj is InstructionsBodyNode ib)
            {
                result = new Extension(context.start.ToLineInfo(), _syntaxStack.PopString(),
                    ExtensionKind.Instructions);
                result.InstructionsBody = ib;
            }
            else
            {
                var path = _syntaxStack.PopString();
                var extensionName = path.Split('.')[0];
                result = new Extension(context.start.ToLineInfo(), extensionName);
                result.Path = path;
            }

            // Вот тут мы должны сделать следующее: 
            // 1) Установить, какой хендлер это обрабатывает
            // 2) Вызвать обработчик. Он должен переопределить дерево вызовов.

            //_syntaxStack.Push();


            if (!ExtensionManager.Managers.TryGetValue(result.ExtensionName, out var ext))
            {
                throw new Exception($"The extension {result.ExtensionName} not found or not loaded");
            }

            //TODO: Закончить разработку расширений
            //_syntaxStack.Push();
            return null;
        }

        public override object VisitExpressionFactor(ZSharpParser.ExpressionFactorContext context)
        {
            base.VisitExpressionFactor(context);


            if (context.expressionFactor() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.GT() != null) opType = BinaryOperatorType.GreaterThen;
                if (context.LT() != null) opType = BinaryOperatorType.LessThen;
                if (context.OP_EQ() != null) opType = BinaryOperatorType.Equal;
                if (context.OP_NE() != null) opType = BinaryOperatorType.NotEqual;
                if (context.PERCENT() != null) opType = BinaryOperatorType.Modulo;
                if (context.OP_GT() != null) opType = BinaryOperatorType.GraterOrEqualTo;
                if (context.OP_LE() != null) opType = BinaryOperatorType.LessOrEqualTo;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType));
            }

            return null;
        }

        public override object VisitExpressionBinary(ZSharpParser.ExpressionBinaryContext context)
        {
            base.VisitExpressionBinary(context);

            if (context.expressionBinary() != null)
            {
                if (context.OP_AND() != null)
                    _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                        _syntaxStack.PopExpression(),
                        BinaryOperatorType.And));
                if (context.OP_OR() != null)
                    _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                        _syntaxStack.PopExpression(),
                        BinaryOperatorType.Or));
            }

            return null;
        }

        public override object VisitAssigment(ZSharpParser.AssigmentContext context)
        {
            base.VisitAssigment(context);

            Statement result;

            if (context.indexExpression != null)
                result = new Assignment(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    _syntaxStack.PopString());
            else if (context.OP_INC() != null)
            {
                result = new PostIncrementStatement(context.start.ToLineInfo(), _syntaxStack.PopString());
            }
            else if (context.OP_DEC() != null)
            {
                result = new PostDecrementStatement(context.start.ToLineInfo(), _syntaxStack.PopString());
            }
            else
                result = new Assignment(context.start.ToLineInfo(), _syntaxStack.PopExpression(), null,
                    _syntaxStack.PopString());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitStatement(ZSharpParser.StatementContext context)
        {
            base.VisitStatement(context);

            AstNode result = null;

            if (context.RETURN() != null)
            {
                if (context.returnExpression == null)
                    result = new Return(context.start.ToLineInfo(), null);
                else
                    result = new Return(context.start.ToLineInfo(), _syntaxStack.PopExpression());

                _syntaxStack.PeekType<IList>().Add(result);
            }
            else
            {
                _syntaxStack.PeekType<IList>().Add(_syntaxStack.PopStatement());
                //_syntaxStack.PeekType<IList>().Add(new Return(null));
            }

            return result;
        }

        public override object VisitInstructionsOrSingleStatement(
            ZSharpParser.InstructionsOrSingleStatementContext context)
        {
            var sc = new StatementCollection();

            if (context.statement() != null)
                _syntaxStack.Push(sc);

            base.VisitInstructionsOrSingleStatement(context);

            if (context.statement() != null)
            {
                _syntaxStack.Pop();
                _syntaxStack.Push(new InstructionsBodyNode(sc));
            }

            return null;
        }

        public override object VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            InstructionsBodyNode @else = null;

            if (context.ELSE() != null)
            {
                @else = _syntaxStack.PopInstructionsBody();
            }

            var result = new If(context.start.ToLineInfo(), @else, _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopStatement(),
                _syntaxStack.PopExpression(), _syntaxStack.PopStatement());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());
            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitTryStatement(ZSharpParser.TryStatementContext context)
        {
            base.VisitTryStatement(context);

            InstructionsBodyNode @catch = null, @finally = null;

            if (context.finallyExp != null)
            {
                @finally = _syntaxStack.PopInstructionsBody();
            }

            if (context.catchExp != null)
            {
                @catch = _syntaxStack.PopInstructionsBody();
            }

            var tryNode = new Try(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(), @catch, @finally);

            _syntaxStack.Push(tryNode);

            return tryNode;
        }
    }
}