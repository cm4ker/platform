using System;
using System.Collections;
using System.Text.RegularExpressions;
using PrototypePlatformLanguage.AST.Definitions;
using PrototypePlatformLanguage.AST.Definitions.Functions;
using PrototypePlatformLanguage.AST.Infrastructure;
using PrototypePlatformLanguage.AST.Definitions.Expression;
using Type = PrototypePlatformLanguage.AST.Definitions.Type;

namespace PrototypePlatformLanguage.AST
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
                    result = new Literal(text.Substring(1, text.Length - 2), LiteralType.String);
                else
                    result = new Literal(text.Substring(2, text.Length - 3), LiteralType.String);
            }
            else if (context.boolean_literal() != null) result = new Literal(context.GetText(), LiteralType.Boolean);
            else if (context.INTEGER_LITERAL() != null) result = new Literal(context.GetText(), LiteralType.Integer);
            else if (context.REAL_LITERAL() != null) result = new Literal(context.GetText(), LiteralType.Real);
            else if (context.CHARACTER_LITERAL() != null)
                result = new Literal(context.GetText().Substring(1, 1), LiteralType.Character);

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
                result = new Variable(_syntaxStack.PopExpression(), context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());


            _syntaxStack.Push(result);
            return result;
        }

        public override object VisitFunctionDeclaration(ZSharpParser.FunctionDeclarationContext context)
        {
            base.VisitFunctionDeclaration(context);
            object result = null;
            ParameterCollection pc = null;

            var body = _syntaxStack.PopMethodBody();

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
            base.VisitParameters(context);
            return null;
        }


        public override object VisitParameter(ZSharpParser.ParameterContext context)
        {
            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            _syntaxStack.PeekCollection()
                .Add(new Parameter(context.IDENTIFIER().GetText(), _syntaxStack.PopType(), passMethod));


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
                _syntaxStack.Push(new Name(_syntaxStack.PopString()));
            return null;
        }

        public override object VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
        {
            base.VisitExpressionUnary(context);

            if (context.PLUS() != null)
                _syntaxStack.Push(new UnaryExpression(null, _syntaxStack.PopExpression(), UnaryOperatorType.Positive));
            if (context.MINUS() != null)
                _syntaxStack.Push(new UnaryExpression(null, _syntaxStack.PopExpression(), UnaryOperatorType.Negative));
            if (context.BANG() != null)
                _syntaxStack.Push(new UnaryExpression(null, _syntaxStack.PopExpression(), UnaryOperatorType.Not));
            if (context.expression() != null)
                _syntaxStack.Push(new UnaryExpression(_syntaxStack.PopExpression(), _syntaxStack.PopExpression(),
                    UnaryOperatorType.Indexer));

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
    }
}