using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Sre;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Extension;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST
{
    public class ZLanguageVisitor : ZSharpParserBaseVisitor<AstNode>
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

        public override AstNode VisitEntryPoint(ZSharpParser.EntryPointContext context)
        {
            _syntaxStack.Clear();

            var cu = new CompilationUnit(context.start.ToLineInfo());
            _syntaxStack.Push(cu);


            base.VisitEntryPoint(context);

            return cu;
        }

        public override AstNode VisitUsingDefinition(ZSharpParser.UsingDefinitionContext context)
        {
            base.VisitUsingDefinition(context);
            _syntaxStack.PeekType<CompilationUnit>().Namespaces.Add(_syntaxStack.PopString());

            return null;
        }

        public override AstNode VisitModuleDefinition(ZSharpParser.ModuleDefinitionContext context)
        {
            base.VisitModuleDefinition(context);

            Module result = new Module(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.IDENTIFIER().GetText());

            _syntaxStack.PeekType<CompilationUnit>().TypeEntities.Add(result);

            return result;
        }

        public override AstNode VisitTypeDefinition(ZSharpParser.TypeDefinitionContext context)
        {
            base.VisitTypeDefinition(context);

            var result = new Class(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.IDENTIFIER().GetText());

            _syntaxStack.PeekType<CompilationUnit>().TypeEntities.Add(result);

            return result;
        }


        public override AstNode VisitModuleBody(ZSharpParser.ModuleBodyContext context)
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

        public override AstNode VisitFieldDeclaration(ZSharpParser.FieldDeclarationContext context)
        {
            base.VisitFieldDeclaration(context);
            Field f = new Field(context.start.ToLineInfo(), _syntaxStack.PopString(), _syntaxStack.PopType());
            _syntaxStack.PeekCollection().Add(f);
            return f;
        }

        public override AstNode VisitMultitype(ZSharpParser.MultitypeContext context)
        {
            var marker = new object();
            _syntaxStack.Push(marker);
            base.VisitMultitype(context);
            var tc = new TypeCollection();
            _syntaxStack.PopUntil(marker, tc);
            return null;
        }

        public override AstNode VisitPropertyDeclaration(ZSharpParser.PropertyDeclarationContext context)
        {
            base.VisitPropertyDeclaration(context);

            InstructionsBodyNode set = null, get = null;
            if (context.setInst != null)
            {
                set = _syntaxStack.PopInstructionsBody();
            }

            if (context.getInst != null)
            {
                get = _syntaxStack.PopInstructionsBody();
            }

            Property p =
                new Property(context.start.ToLineInfo(), _syntaxStack.PopString(), _syntaxStack.PopType(),
                    context.GET() != null, context.SET() != null)
                {
                    Getter = get,
                    Setter = set
                };

            _syntaxStack.PeekCollection().Add(p);
            return p;
        }

        public override AstNode VisitTypeBody(ZSharpParser.TypeBodyContext context)
        {
            _syntaxStack.Push(new MemberCollection());
            base.VisitTypeBody(context);

            TypeBody result;

            if (context.ChildCount == 0)
                result = new TypeBody(null);
            else
                result = new TypeBody((MemberCollection) _syntaxStack.Pop());

            _syntaxStack.Push(result);
            return result;
        }

        public override AstNode VisitAttributes(ZSharpParser.AttributesContext context)
        {
            _syntaxStack.Push(new AttributeCollection());
            base.VisitAttributes(context);

            return null;
        }

        public override AstNode VisitAttribute(ZSharpParser.AttributeContext context)
        {
            base.VisitAttribute(context);

            ArgumentCollection ac = null;

            if (context.arguments() != null)
                ac = (ArgumentCollection) _syntaxStack.Pop();

            var result = new AttributeNode(context.start.ToLineInfo(), ac, _syntaxStack.PopType());
            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override AstNode VisitName(ZSharpParser.NameContext context)
        {
            _syntaxStack.Push(context.GetText());

            return null;
        }

        public override AstNode VisitStructureType(ZSharpParser.StructureTypeContext context)
        {
            var result = new SingleTypeNode(context.start.ToLineInfo(), context.GetText());

            _syntaxStack.Push(result);
            return result;
        }

        public override AstNode VisitPrimitiveType(ZSharpParser.PrimitiveTypeContext context)
        {
            IType t = null;
            if (context.STRING() != null) t = _sb.String;
            else if (context.INT() != null) t = _sb.Int;
            else if (context.BOOL() != null) t = _sb.Bool;
            else if (context.DOUBLE() != null) t = _sb.Double;
            else if (context.CHAR() != null) t = _sb.Char;
            else if (context.VOID() != null) t = _sb.Void;

            if (t == null)
                throw new Exception("Unknown primitive type");
            var result = new SingleTypeNode(context.start.ToLineInfo(), t);

            _syntaxStack.Push(result);

            return result;
        }

        public override AstNode VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = new SingleTypeNode(context.start.ToLineInfo(),
                new UnknownArrayType(context.GetText(), _syntaxStack.PopType().Type));

            _syntaxStack.Push(result);
            return result;
        }

        public override AstNode VisitLiteral(ZSharpParser.LiteralContext context)
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
                    result = new Literal(li, text.Substring(1, text.Length - 2), new SingleTypeNode(li, _sb.String));
                else
                    result = new Literal(li, text.Substring(2, text.Length - 3), new SingleTypeNode(li, _sb.String));
            }
            else if (context.boolean_literal() != null)
                result = new Literal(li, context.GetText(), new SingleTypeNode(li, _sb.Bool));
            else if (context.INTEGER_LITERAL() != null)
                result = new Literal(li, context.GetText(), new SingleTypeNode(li, _sb.Int));
            else if (context.REAL_LITERAL() != null)
                result = new Literal(li, context.GetText(), new SingleTypeNode(li, _sb.Double));
            else if (context.CHARACTER_LITERAL() != null)
                result = new Literal(li, context.GetText().Substring(1, 1), new SingleTypeNode(li, _sb.Char));

            //TODO: Не обработанным остался HEX INTEGER LITERAL его необходимо доделать

            if (result == null)
                throw new Exception("Unknown literal");

            _syntaxStack.Push(result);

            return result;
        }

        public override AstNode VisitVariableDeclaration(ZSharpParser.VariableDeclarationContext context)
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

        public override AstNode VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                _syntaxStack.PopType());

            _syntaxStack.Push(result);

            return result;
        }

        public override AstNode VisitFunctionDeclaration(ZSharpParser.FunctionDeclarationContext context)
        {
            base.VisitFunctionDeclaration(context);
            Function result = null;
            ParameterCollection pc = null;
            AttributeCollection ac = new AttributeCollection();
            var body = _syntaxStack.PopInstructionsBody();

            if (context.parameters() != null)
            {
                pc = (ParameterCollection) _syntaxStack.Pop();
            }


            var type = _syntaxStack.PopType();

            var funcName = context.IDENTIFIER().GetText();

            if (context.attributes() != null)
            {
                ac = (AttributeCollection) _syntaxStack.Pop();
            }

            result = new Function(context.start.ToLineInfo(), body, pc, funcName, type, ac);

            if (context.accessModifier()?.PUBLIC() != null)
            {
                result.IsPublic = true;
            }

            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override AstNode VisitInstructionsBody(ZSharpParser.InstructionsBodyContext context)
        {
            base.VisitInstructionsBody(context);
            var sc = (StatementCollection) _syntaxStack.Pop();
            _syntaxStack.Push(new InstructionsBodyNode(sc));
            return null;
        }

        public override AstNode VisitParameters(ZSharpParser.ParametersContext context)
        {
            _syntaxStack.Push(new ParameterCollection());
            return base.VisitParameters(context);
        }


        public override AstNode VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = _syntaxStack.PeekCollection();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new Parameter(context.start.ToLineInfo(), context.IDENTIFIER().GetText(),
                _syntaxStack.PopType(), passMethod);

            paramList.Add(parameter);

            return null;
        }


        public override AstNode VisitArguments(ZSharpParser.ArgumentsContext context)
        {
            _syntaxStack.Push(new ArgumentCollection());

            base.VisitArguments(context);

            return null;
        }

        public override AstNode VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);
            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new Argument(context.start.ToLineInfo(), _syntaxStack.PopExpression(), passMethod);

            _syntaxStack.PeekCollection().Add(result);
            return result;
        }

        public override AstNode VisitFunctionCall(ZSharpParser.FunctionCallContext context)
        {
            base.VisitFunctionCall(context);

            var result = new CallStatement(context.start.ToLineInfo(), (ArgumentCollection) _syntaxStack.Pop(),
                _syntaxStack.PopString());


            _syntaxStack.Push(result);

            return result;
        }


        public override AstNode VisitFunctionCallExpression(ZSharpParser.FunctionCallExpressionContext context)
        {
            base.VisitFunctionCallExpression(context);

            var callStatement = (CallStatement) _syntaxStack.Pop();

            var result = new Call(context.start.ToLineInfo(), callStatement.Arguments, callStatement.Name);

            _syntaxStack.Push(result);
            return null;
        }

        public override AstNode VisitStatements(ZSharpParser.StatementsContext context)
        {
            _syntaxStack.Push(new StatementCollection());
            base.VisitStatements(context);
            return null;
        }

        public override AstNode VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            return null;
        }


        public override AstNode VisitExpressionAtom(ZSharpParser.ExpressionAtomContext context)
        {
            base.VisitExpressionAtom(context);

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

        public override AstNode VisitExpressionPostfix(ZSharpParser.ExpressionPostfixContext context)
        {
            base.VisitExpressionPostfix(context);
            var li = context.start.ToLineInfo();
            if (context.indexerExpression != null)
                _syntaxStack.Push(new IndexerExpression(li, _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression()));

            return null;
        }

        public override AstNode VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
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


            return null;
        }

        public override AstNode VisitExpressionMultiplicative(ZSharpParser.ExpressionMultiplicativeContext context)
        {
            base.VisitExpressionMultiplicative(context);

            if (context.expressionMultiplicative() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.PERCENT() != null) opType = BinaryOperatorType.Modulo;
                if (context.DIV() != null) opType = BinaryOperatorType.Divide;
                if (context.STAR() != null) opType = BinaryOperatorType.Multiply;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), opType));
            }

            return null;
        }

        public override AstNode VisitExtensionExpression(ZSharpParser.ExtensionExpressionContext context)
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

        public override AstNode VisitExpressionEquality(ZSharpParser.ExpressionEqualityContext context)
        {
            base.VisitExpressionEquality(context);

            if (context.expressionEquality() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;


                if (context.OP_EQ() != null) opType = BinaryOperatorType.Equal;
                if (context.OP_NE() != null) opType = BinaryOperatorType.NotEqual;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType));
            }

            return null;
        }

        public override AstNode VisitExpressionRelational(ZSharpParser.ExpressionRelationalContext context)
        {
            base.VisitExpressionRelational(context);

            if (context.expressionRelational() != null)
            {
                BinaryOperatorType opType = BinaryOperatorType.None;

                if (context.GT() != null) opType = BinaryOperatorType.GreaterThen;
                if (context.LT() != null) opType = BinaryOperatorType.LessThen;
                if (context.OP_GT() != null) opType = BinaryOperatorType.GraterOrEqualTo;
                if (context.OP_LE() != null) opType = BinaryOperatorType.LessOrEqualTo;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType));
            }


            return null;
        }

        public override AstNode VisitExpressionAdditive(ZSharpParser.ExpressionAdditiveContext context)
        {
            base.VisitExpressionAdditive(context);

            if (context.expressionAdditive() != null)
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

        public override AstNode VisitExpressionBinary(ZSharpParser.ExpressionBinaryContext context)
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

        public override AstNode VisitAssigment(ZSharpParser.AssigmentContext context)
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

        public override AstNode VisitStatement(ZSharpParser.StatementContext context)
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

        public override AstNode VisitInstructionsOrSingleStatement(
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

        public override AstNode VisitIfStatement(ZSharpParser.IfStatementContext context)
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

        public override AstNode VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopStatement(),
                _syntaxStack.PopExpression(), _syntaxStack.PopStatement());

            _syntaxStack.Push(result);

            return result;
        }

        public override AstNode VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());
            _syntaxStack.Push(result);

            return result;
        }

        public override AstNode VisitTryStatement(ZSharpParser.TryStatementContext context)
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