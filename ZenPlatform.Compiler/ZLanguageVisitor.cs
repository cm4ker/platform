using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Npgsql.NameTranslation;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Extension;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using Attribute = ZenPlatform.Language.Ast.Definitions.Attribute;
using Expression = ZenPlatform.Language.Ast.Definitions.Expression;

namespace ZenPlatform.Compiler
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

            var typeList = new List<TypeEntity>();

            _syntaxStack.Push(typeList);

            var usings = new List<NamespaceBase>();

            foreach (var atd in context.aliasingTypeDefinition())
            {
                usings.Add((NamespaceBase) Visit(atd));
            }

            foreach (var u in context.usingDefinition())
            {
                usings.Add((NamespaceBase) Visit(u));
            }

            base.VisitEntryPoint(context);

            var cu = new CompilationUnit(context.start.ToLineInfo(), usings, typeList);

            return cu;
        }

        public override SyntaxNode VisitAliasingTypeDefinition(ZSharpParser.AliasingTypeDefinitionContext context)
        {
            return new ClassNamespace(context.start.ToLineInfo(),
                context.typeName().GetText(), context.alias.GetText());
        }

        public override SyntaxNode VisitUsingDefinition(ZSharpParser.UsingDefinitionContext context)
        {
            base.VisitUsingDefinition(context);
            return new Namespace(context.start.ToLineInfo(), _syntaxStack.PopString());
        }

        public override SyntaxNode VisitModuleDefinition(ZSharpParser.ModuleDefinitionContext context)
        {
            base.VisitModuleDefinition(context);

            Module result = new Module(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.typeName().GetText());

            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override SyntaxNode VisitTypeDefinition(ZSharpParser.TypeDefinitionContext context)
        {
            base.VisitTypeDefinition(context);

            var result = new Class(context.start.ToLineInfo(), _syntaxStack.PopTypeBody(),
                context.typeName().IDENTIFIER().GetText());

            result.Namespace = context.typeName().@namespace()?.GetText();
            
            _syntaxStack.PeekCollection().Add(result);

            return result;
        }


        public override SyntaxNode VisitModuleBody(ZSharpParser.ModuleBodyContext context)
        {
            _syntaxStack.Push(new MemberCollection());
            base.VisitModuleBody(context);

            TypeBody result;

            if (context.ChildCount == 0)
                result = new TypeBody(null);
            else
                result = new TypeBody(_syntaxStack.PopList<Member>().ToImmutableList());

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitFieldDeclaration(ZSharpParser.FieldDeclarationContext context)
        {
            base.VisitFieldDeclaration(context);
            Field f = new Field(context.start.ToLineInfo(), _syntaxStack.PopString(), _syntaxStack.PopType());
            _syntaxStack.PeekCollection().Add(f);
            return f;
        }

        public override SyntaxNode VisitMultitype(ZSharpParser.MultitypeContext context)
        {
            var marker = new object();
            _syntaxStack.Push(marker);
            base.VisitMultitype(context);
            var tc = new TypeCollection();
            _syntaxStack.PopUntil(marker, tc);
            var result = new UnionTypeSyntax(context.start.ToLineInfo(), tc);
            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitPropertyDeclaration(ZSharpParser.PropertyDeclarationContext context)
        {
            base.VisitPropertyDeclaration(context);

            Block set = null, get = null;
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

        public override SyntaxNode VisitTypeBody(ZSharpParser.TypeBodyContext context)
        {
            _syntaxStack.Push(new MemberCollection());
            base.VisitTypeBody(context);

            TypeBody result;

            if (context.ChildCount == 0)
                result = new TypeBody(null);
            else
                result = new TypeBody(_syntaxStack.PopList<Member>().ToImmutableList());

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitAttributes(ZSharpParser.AttributesContext context)
        {
            _syntaxStack.Push(new AttributeCollection());
            base.VisitAttributes(context);

            return null;
        }

        public override SyntaxNode VisitAttribute(ZSharpParser.AttributeContext context)
        {
            base.VisitAttribute(context);

            ArgumentCollection ac = null;

            if (context.arguments() != null)
                ac = (ArgumentCollection) _syntaxStack.Pop();

            var result = new Attribute(context.start.ToLineInfo(), ac, _syntaxStack.PopType() as SingleTypeSyntax);
            _syntaxStack.PeekCollection().Add(result);

            return result;
        }

        public override SyntaxNode VisitName(ZSharpParser.NameContext context)
        {
            _syntaxStack.Push(context.GetText());

            return null;
        }

        public override SyntaxNode VisitStructureType(ZSharpParser.StructureTypeContext context)
        {
            var result = new SingleTypeSyntax(context.start.ToLineInfo(), context.GetText(), TypeNodeKind.Type);

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitPrimitiveType(ZSharpParser.PrimitiveTypeContext context)
        {
            TypeNodeKind t = TypeNodeKind.Unknown;
            if (context.STRING() != null) t = TypeNodeKind.String;
            else if (context.INT() != null) t = TypeNodeKind.Int;
            else if (context.OBJECT() != null) t = TypeNodeKind.Object;
            else if (context.BOOL() != null) t = TypeNodeKind.Boolean;
            else if (context.DOUBLE() != null) t = TypeNodeKind.Double;
            else if (context.CHAR() != null) t = TypeNodeKind.Char;
            else if (context.VOID() != null) t = TypeNodeKind.Void;

            if (t == TypeNodeKind.Unknown)
                throw new Exception("Unknown primitive type");
            var result = new PrimitiveTypeSyntax(context.start.ToLineInfo(), t);

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            var result = new ArrayTypeSyntax(context.start.ToLineInfo(), _syntaxStack.PopType());

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitLiteral(ZSharpParser.LiteralContext context)
        {
            Literal result = null;
            ILineInfo li = context.start.ToLineInfo();
            if (context.string_literal() != null)
            {
                //Строки парсятся сюда вместе с кавычками и чтобы их убрать приходится
                //заниматься таким вот извращением

                var text = context.string_literal().REGULAR_STRING()?.GetText() ??
                           context.string_literal().VERBATIUM_STRING()?.GetText();

                text = Regex.Unescape(text ?? throw new NullReferenceException());

                if (context.string_literal().REGULAR_STRING() != null)
                    result = new Literal(li, text.Substring(1, text.Length - 2),
                        new PrimitiveTypeSyntax(li, TypeNodeKind.String));
                else
                    result = new Literal(li, text.Substring(2, text.Length - 3),
                        new PrimitiveTypeSyntax(li, TypeNodeKind.String));

                result.ObjectiveValue = result.Value;
            }
            else if (context.boolean_literal() != null)
            {
                result = new Literal(li, context.GetText(), new PrimitiveTypeSyntax(li, TypeNodeKind.Boolean));
                result.ObjectiveValue = bool.Parse(result.Value);
            }
            else if (context.INTEGER_LITERAL() != null)
            {
                result = new Literal(li, context.GetText(), new PrimitiveTypeSyntax(li, TypeNodeKind.Int));
                result.ObjectiveValue = int.Parse(result.Value);
            }
            else if (context.REAL_LITERAL() != null)
            {
                result = new Literal(li, context.GetText(), new PrimitiveTypeSyntax(li, TypeNodeKind.Double));
                result.ObjectiveValue = double.Parse(result.Value);
            }
            else if (context.CHARACTER_LITERAL() != null)
            {
                result = new Literal(li, context.GetText().Substring(1, 1),
                    new PrimitiveTypeSyntax(li, TypeNodeKind.Char));
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
            if (context.expression() == null)
                result = new Variable(context.start.ToLineInfo(), null, context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());
            else
                result = new Variable(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    context.IDENTIFIER().GetText(),
                    _syntaxStack.PopType());

            _syntaxStack.Push(result);
            return result;
        }

        public override SyntaxNode VisitCastExpression(ZSharpParser.CastExpressionContext context)
        {
            base.VisitCastExpression(context);

            var result = new CastExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                _syntaxStack.PopType(), UnaryOperatorType.Cast);

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitLookupExpression(ZSharpParser.LookupExpressionContext context)
        {
            base.VisitLookupExpression(context);

            var result = new LookupExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitFunctionDeclaration(ZSharpParser.FunctionDeclarationContext context)
        {
            base.VisitFunctionDeclaration(context);
            Function result = null;
            ParameterCollection pc = new ParameterCollection();
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

            result = new Function(context.start.ToLineInfo(), body, pc, ac, funcName, type);

            if (context.accessModifier()?.PUBLIC() != null)
            {
                result.IsPublic = true;
            }

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
            var sc = (StatementCollection) _syntaxStack.Pop();
            _syntaxStack.Push(new Block(sc));
            return null;
        }

        public override SyntaxNode VisitParameters(ZSharpParser.ParametersContext context)
        {
            _syntaxStack.Push(new ParameterCollection());
            return base.VisitParameters(context);
        }


        public override SyntaxNode VisitParameter(ZSharpParser.ParameterContext context)
        {
            var paramList = _syntaxStack.PeekCollection();

            base.VisitParameter(context);

            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;

            var parameter = new Parameter(context.start.ToLineInfo(), context.IDENTIFIER().GetText(),
                _syntaxStack.PopType(), passMethod);

            paramList.Add(parameter);

            return null;
        }


        public override SyntaxNode VisitArguments(ZSharpParser.ArgumentsContext context)
        {
            _syntaxStack.Push(new ArgumentCollection());

            base.VisitArguments(context);

            return null;
        }

        public override SyntaxNode VisitArgument(ZSharpParser.ArgumentContext context)
        {
            base.VisitArgument(context);
            var passMethod = context.REF() != null ? PassMethod.ByReference : PassMethod.ByValue;
            var result = new Argument(context.start.ToLineInfo(), _syntaxStack.PopExpression(), passMethod);

            _syntaxStack.PeekCollection().Add(result);
            return result;
        }

        public override SyntaxNode VisitFunctionCall(ZSharpParser.FunctionCallContext context)
        {
            base.VisitFunctionCall(context);

            var result = new Call(context.start.ToLineInfo(), _syntaxStack.PopList<Argument>().ToImmutableList(),
                _syntaxStack.PopString(), null);

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitStatements(ZSharpParser.StatementsContext context)
        {
            _syntaxStack.Push(new StatementCollection());
            base.VisitStatements(context);
            return null;
        }

        public override SyntaxNode VisitExpression(ZSharpParser.ExpressionContext context)
        {
            base.VisitExpression(context);

            return null;
        }


        public override SyntaxNode VisitExpressionAtom(ZSharpParser.ExpressionAtomContext context)
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
                        _syntaxStack.Push(new GetFieldExpression(_syntaxStack.PopExpression(), str));
                    }
                }
                else
                {
                    _syntaxStack.Push(new Name(li, identifier[0]));
                }
            }

            return null;
        }

        public override SyntaxNode VisitExpressionPostfix(ZSharpParser.ExpressionPostfixContext context)
        {
            base.VisitExpressionPostfix(context);
            var li = context.start.ToLineInfo();
            if (context.indexerExpression != null)
                _syntaxStack.Push(new IndexerExpression(li, _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(), UnaryOperatorType.Indexer));

            return null;
        }

        public override SyntaxNode VisitExpressionUnary(ZSharpParser.ExpressionUnaryContext context)
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

        public override SyntaxNode VisitExpressionMultiplicative(ZSharpParser.ExpressionMultiplicativeContext context)
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

//        public override SyntaxNode VisitExtensionExpression(ZSharpParser.ExtensionExpressionContext context)
//        {
//            base.VisitExtensionExpression(context);
//            Extension result;
//
//            var extensionObj = _syntaxStack.Pop();
//            if (extensionObj is Block ib)
//            {
//                result = new Extension(context.start.ToLineInfo(), _syntaxStack.PopString(),
//                    ExtensionKind.Instructions);
//                result.Block = ib;
//            }
//            else
//            {
//                var path = _syntaxStack.PopString();
//                var extensionName = path.Split('.')[0];
//                result = new Extension(context.start.ToLineInfo(), extensionName);
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

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType));
            }

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
                if (context.OP_GT() != null) opType = BinaryOperatorType.GraterOrEqualTo;
                if (context.OP_LE() != null) opType = BinaryOperatorType.LessOrEqualTo;

                _syntaxStack.Push(new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType));
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

                var result = new BinaryExpression(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    opType);

                _syntaxStack.Push(result);
            }


            return null;
        }

        public override SyntaxNode VisitExpressionBinary(ZSharpParser.ExpressionBinaryContext context)
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

        public override SyntaxNode VisitAssigment(ZSharpParser.AssigmentContext context)
        {
            base.VisitAssigment(context);

            Expression result;

            if (context.indexExpression != null)
                result = new Assignment(context.start.ToLineInfo(), _syntaxStack.PopExpression(),
                    _syntaxStack.PopExpression(),
                    new Name(null, _syntaxStack.PopString()));
            else if (context.OP_INC() != null)
            {
                result = new PostIncrementExpression(context.start.ToLineInfo(), _syntaxStack.PopString());
            }
            else if (context.OP_DEC() != null)
            {
                result = new PostDecrementExpression(context.start.ToLineInfo(), _syntaxStack.PopString());
            }
            else
                result = new Assignment(context.start.ToLineInfo(), _syntaxStack.PopExpression(), null,
                    new Name(null, _syntaxStack.PopString()));

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitStatement(ZSharpParser.StatementContext context)
        {
            base.VisitStatement(context);

            SyntaxNode result = null;

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
                var node = _syntaxStack.Pop();

                // По умолчанию все операции могут являться выражениями.
                //перед тем как мы будем добавлять их в инструкции нужно обернуть их в инструкцию
                if (node is Expression exp)
                {
                    if (exp is Call c)
                        c.IsStatement = true;

                    node = new ExpressionStatement(exp);
                }

                _syntaxStack.PeekType<IList>().Add(node);
            }

            return result;
        }

        public override SyntaxNode VisitInstructionsOrSingleStatement(
            ZSharpParser.InstructionsOrSingleStatementContext context)
        {
            var sc = new StatementCollection();

            if (context.statement() != null)
                _syntaxStack.Push(sc);

            base.VisitInstructionsOrSingleStatement(context);

            if (context.statement() != null)
            {
                _syntaxStack.Pop();
                _syntaxStack.Push(new Block(sc));
            }

            return null;
        }

        public override SyntaxNode VisitIfStatement(ZSharpParser.IfStatementContext context)
        {
            base.VisitIfStatement(context);

            Block @else = null;

            if (context.ELSE() != null)
            {
                @else = _syntaxStack.PopInstructionsBody();
            }

            var result = new If(context.start.ToLineInfo(), @else, _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitForStatement(ZSharpParser.ForStatementContext context)
        {
            base.VisitForStatement(context);

            var result = new For(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression(),
                _syntaxStack.PopExpression(), _syntaxStack.PopExpression());

            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitWhileStatement(ZSharpParser.WhileStatementContext context)
        {
            base.VisitWhileStatement(context);

            var result = new While(context.start.ToLineInfo(), _syntaxStack.PopInstructionsBody(),
                _syntaxStack.PopExpression());
            _syntaxStack.Push(result);

            return result;
        }

        public override SyntaxNode VisitTryStatement(ZSharpParser.TryStatementContext context)
        {
            base.VisitTryStatement(context);

            Block @catch = null, @finally = null;

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


        public override SyntaxNode VisitGlobalVar(ZSharpParser.GlobalVarContext context)
        {
            base.VisitGlobalVar(context);
            _syntaxStack.Push(new GlobalVar(context.start.ToLineInfo(), _syntaxStack.PopExpression()));
            return null;
        }
    }
}