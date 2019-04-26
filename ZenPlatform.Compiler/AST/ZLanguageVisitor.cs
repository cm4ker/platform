using System;
using System.Collections;
using System.Text.RegularExpressions;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expression;
using ZenPlatform.Compiler.AST.Definitions.Extension;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Infrastructure;
using BinaryExpression = ZenPlatform.Compiler.AST.Definitions.Expression.BinaryExpression;
using Type = ZenPlatform.Compiler.AST.Definitions.Type;

namespace ZenPlatform.Compiler.AST
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<object>
    {
        private SyntaxStack _syntaxStack;

        public ZLanguageVisitor()
        {
            _syntaxStack = new SyntaxStack();
        }

        public override object VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            _syntaxStack.Clear();

            base.VisitEntryPoint(context);

            return _syntaxStack.Pop();
        }

        public override object VisitModuleDefinition(ZSharpParser.ModuleDefinitionContext context)
        {
            base.VisitModuleDefinition(context);

            object result = new Module(_syntaxStack.PopTypeBody(), context.IDENTIFIER().GetText());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitModuleBody(ZSharpParser.ModuleBodyContext context)
        {
            _syntaxStack.Push(new MemberCollection());
            base.VisitModuleBody(context);

            object result;

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
            var result = new Type(context.GetText());
            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitPrimitiveType(ZSharpParser.PrimitiveTypeContext context)
        {
            object result = null;
            if (context.STRING() != null) result = new Type(PrimitiveType.String);
            else if (context.INT() != null) result = new Type(PrimitiveType.Integer);
            else if (context.BOOL() != null) result = new Type(PrimitiveType.Boolean);
            else if (context.DOUBLE() != null) result = new Type(PrimitiveType.Double);
            else if (context.CHAR() != null) result = new Type(PrimitiveType.Character);
            else if (context.VOID() != null) result = new Type(PrimitiveType.Void);

            if (result == null)
                throw new Exception("Unknown primitive type");
            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = Type.CreateArrayFromType(_syntaxStack.PopType());
            _syntaxStack.Push(result);


            return result;
        }

        public override object VisitLiteral(ZSharpParser.LiteralContext context)
        {
            object result = null;
            if (context.string_literal() != null)
            {
                //Строки парсятся сюда вместе с кавычками и чтобы их убрать приходится
                //заниматься таким вот извращением

                var text = context.string_literal().REGULAR_STRING()?.GetText() ??
                           context.string_literal().VERBATIUM_STRING()?.GetText();

                text = Regex.Unescape(text ?? throw new NullReferenceException());

                if (context.string_literal().REGULAR_STRING() != null)
                    result = new Literal(text.Substring(1, text.Length - 2), Type.String);
                else
                    result = new Literal(text.Substring(2, text.Length - 3), Type.String);
            }
            else if (context.boolean_literal() != null) result = new Literal(context.GetText(), Type.Bool);
            else if (context.INTEGER_LITERAL() != null) result = new Literal(context.GetText(), Type.Int);
            else if (context.REAL_LITERAL() != null) result = new Literal(context.GetText(), Type.Double);
            else if (context.CHARACTER_LITERAL() != null)
                result = new Literal(context.GetText().Substring(1, 1), Type.Character);

            //TODO: Не обработанным остался HEX INTEGER LITERAL его необходимо доделать

            if (result == null)
                throw new Exception("Unknown literal");

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitVariableDeclaration(ZSharpParser.VariableDeclarationContext context)
        {
            base.VisitVariableDeclaration(context);

            object result;
            if (context.expression() == null)
                result = new Variable(null, context.IDENTIFIER().GetText(), _syntaxStack.PopType());
            else
                result = new Variable(_syntaxStack.Pop(), context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());


            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpression(_syntaxStack.PopExpression(), _syntaxStack.PopType());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitFunctionDeclaration(ZSharpParser.FunctionDeclarationContext context)
        {
            base.VisitFunctionDeclaration(context);
            object result = null;
            ParameterCollection pc = null;

            var body = _syntaxStack.PopInstructionsBody();

            if (context.parameters() != null)
            {
                pc = (ParameterCollection) _syntaxStack.Pop();
            }

            var type = _syntaxStack.PopType();
            var funcName = context.IDENTIFIER().GetText();

            result = new Function(body, pc, funcName, type);

            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override object VisitInstructionsBody(ZSharpParser.InstructionsBodyContext context)
        {
            base.VisitInstructionsBody(context);
            var sc = (StatementCollection) _syntaxStack.Pop();
            _syntaxStack.Push(new InstructionsBody(sc));
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

            paramList.Add(new Parameter(context.IDENTIFIER().GetText(), _syntaxStack.PopType(), passMethod));

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
            var result = new Argument(_syntaxStack.PopExpression(), passMethod);
            _syntaxStack.PeekCollection().Add(result);
            return result;
        }

        public override object VisitFunctionCall(ZSharpParser.FunctionCallContext context)
        {
            base.VisitFunctionCall(context);

            _syntaxStack.Push(new CallStatement((ArgumentCollection) _syntaxStack.Pop(), _syntaxStack.PopString()));

            return null;
        }


        public override object VisitFunctionCallExpression(ZSharpParser.FunctionCallExpressionContext context)
        {
            base.VisitFunctionCallExpression(context);

            var callStatement = (CallStatement) _syntaxStack.Pop();

            _syntaxStack.Push(new Call(callStatement.Arguments, callStatement.Name));
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

                _syntaxStack.Push(new BinaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                    opType));
            }

            return null;
        }

        public override object VisitExpressionPrimary(ZSharpParser.ExpressionPrimaryContext context)
        {
            base.VisitExpressionPrimary(context);

            if (context.name() != null)
            {
                var identifier = _syntaxStack.PopString().Split('.');

                //Если мы пытаемся получить какое-то свойство у переменной, то мы обязательно должны пометить это как PropertyExpression
                // Похожая механика реализована и в FunctionCall
                if (identifier.Length > 1)
                {
                    _syntaxStack.Push(new Name(identifier[0]));

                    foreach (var str in identifier[1..])
                    {
                        _syntaxStack.Push(new PropertyExpression(_syntaxStack.PopExpression(), str));
                    }
                }
                else
                {
                    _syntaxStack.Push(new Name(identifier[0]));
                }
            }

            return null;
        }

        public override object VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
        {
            base.VisitExpressionUnary(context);

            if (context.PLUS() != null)
                _syntaxStack.Push(new LogicalOrArithmeticExpression(_syntaxStack.PopExpression(),
                    UnaryOperatorType.Positive));
            if (context.MINUS() != null)
                _syntaxStack.Push(new LogicalOrArithmeticExpression(_syntaxStack.PopExpression(),
                    UnaryOperatorType.Negative));
            if (context.BANG() != null)
                _syntaxStack.Push(
                    new LogicalOrArithmeticExpression(_syntaxStack.PopExpression(), UnaryOperatorType.Not));
            if (context.expression() != null)
                _syntaxStack.Push(new IndexerExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression()));

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

                _syntaxStack.Push(new BinaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                    opType));
            }

            return null;
        }

        public override object VisitExtensionExpression(ZSharpParser.ExtensionExpressionContext context)
        {
            base.VisitExtensionExpression(context);
            Extension result;

            var extensionObj = _syntaxStack.Pop();
            if (extensionObj is InstructionsBody ib)
            {
                result = new Extension(_syntaxStack.PopString(), ExtensionKind.Instructions);
                result.InstructionsBody = ib;
            }
            else
            {
                var path = _syntaxStack.PopString();
                var extensionName = path.Split('.')[0];
                result = new Extension(extensionName);
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

                _syntaxStack.Push(new BinaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
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
                    _syntaxStack.Push(new BinaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                        BinaryOperatorType.And));
                if (context.OP_OR() != null)
                    _syntaxStack.Push(new BinaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                        BinaryOperatorType.Or));
            }

            return null;
        }

        public override object VisitAssigment(ZSharpParser.AssigmentContext context)
        {
            base.VisitAssigment(context);

            Statement result;

            if (context.indexExpression != null)
                result = new Assignment(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopString());
            else if (context.OP_INC() != null)
            {
                result = new PostIncrementStatement(new Name(_syntaxStack.PopString()));
            }
            else
                result = new Assignment(_syntaxStack.PopExpression(), null, _syntaxStack.PopString());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitStatement(ZSharpParser.StatementContext context)
        {
            base.VisitStatement(context);

            object result;

            if (context.RETURN().Length != 0)
            {
                if (context.expression() == null)
                    result = new Return(null);
                else
                    result = new Return(_syntaxStack.PopExpression());
                _syntaxStack.PeekType<IList>().Add(result);
            }
            else if (context.expression() != null)
            {
                _syntaxStack.PeekType<IList>().Add(_syntaxStack.PopStatement());
                //_syntaxStack.PeekType<IList>().Add(new Return(null));
            }

            return null;
        }

        public override object VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            InstructionsBody @else = null;

            if (context.ELSE() != null)
            {
                @else = _syntaxStack.PopInstructionsBody();
            }

            var result = new If(@else, _syntaxStack.PopInstructionsBody(), _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(_syntaxStack.PopInstructionsBody(), _syntaxStack.PopStatement(),
                _syntaxStack.PopExpression(), _syntaxStack.PopStatement());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(_syntaxStack.PopInstructionsBody(), _syntaxStack.PopExpression());
            _syntaxStack.Push(result);
            return result;
        }
    }
}