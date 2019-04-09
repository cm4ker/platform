using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PrototypePlatformLanguage.AST
{
    public class Semantics
    {
        public static void Apply(Production production, SyntaxStack stack)
        {
            switch (production.m_ID)
            {
                case 52: // <Statement> ::= return <Expression> ;
                    // Create return object.
                    stack.Pop(1);
                    stack.Remove(1);
                    stack.Push(new Return(stack.PopExpression()));
                    break;
                case 53: // <Statement> ::= return ;
                    // Create return object.
                    stack.Pop(2);
                    stack.Push(new Return(null));
                    break;
                case 54: // <Statement> ::= if '(' <Expression> ')' <Body>
                    // Create if statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(2);
                    stack.Push(new If(null, stack.PopMethodBody(), stack.PopExpression()));
                    break;
                case 55: // <Statement> ::= if '(' <Expression> ')' <Body> else <Body>
                    // Create if statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(3);
                    stack.Remove(3);
                    stack.Push(new If(stack.PopMethodBody(), stack.PopMethodBody(), stack.PopExpression()));
                    break;
                case 56: // <Statement> ::= while '(' <Expression> ')' <Body>
                    // Create while statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(2);
                    stack.Push(new While(stack.PopMethodBody(), stack.PopExpression()));
                    break;
                case 57: // <Statement> ::= do <Body> while '(' <Expression> ')'
                    /// Create do statement.
                    stack.Pop(1);
                    stack.Remove(1);
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Push(new Do(stack.PopExpression(), stack.PopMethodBody()));
                    break;
                case 58: // <Statement> ::= for '(' <Assignment> ; <Expression> ; <Assignment> ')' <Body>
                    // Create for statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(3);
                    stack.Remove(4);
                    stack.Remove(4);
                    stack.Push(new For(stack.PopMethodBody(), stack.PopAssignment(), stack.PopExpression(),
                        stack.PopAssignment()));
                    break;
                case 59: // <Assignment> ::= set <Name> = <Expression>
                    // Create assignment statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Push(new Assignment(stack.PopExpression(), null, stack.PopString()));
                    break;
                case 60: // <Assignment> ::= set <Name> [ <Expression> ] = <Expression>
                    // Create assignment statement.
                    stack.Remove(1);
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(3);
                    stack.Push(new Assignment(stack.PopExpression(), stack.PopExpression(), stack.PopString()));
                    break;
                case 61: // <Assignment> ::= set <Name> '++'
                    // TO DO:
                    break;
                case 62: // <Assignment> ::= set <Name> '--'
                    // TO DO:
                    break;
                case 63: // <Expression> ::= <Expression_Term>
                    // !!! DO NOTHING !!!
                    break;
                case 64: // <Expression> ::= <Expression> '+' <Expression_Term>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Add));
                    break;
                case 65: // <Expression> ::= <Expression> '-' <Expression_Term>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Subtract));
                    break;
                case 66: // <Expression_Term> ::= <Expression_Factor>
                    // !!! DO NOTHING !!!
                    break;
                case 67: // <Expression_Term> ::= <Expression_Term> '*' <Expression_Factor>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Multiply));
                    break;
                case 68: // <Expression_Term> ::= <Expression_Term> / <Expression_Factor>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Divide));
                    break;
                case 69: // <Expression_Factor> ::= <Expression_Binary>
                    // !!! DO NOTHING !!!
                    break;
                case 70: // <Expression_Factor> ::= <Expression_Factor> % <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Modulo));
                    break;
                case 71: // <Expression_Factor> ::= <Expression_Factor> '>' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.GreaterThen));
                    break;
                case 72: // <Expression_Factor> ::= <Expression_Factor> '<' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.LessThen));
                    break;
                case 73: // <Expression_Factor> ::= <Expression_Factor> '>=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.GraterOrEqualTo));
                    break;
                case 74: // <Expression_Factor> ::= <Expression_Factor> '<=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.LessOrEqualTo));
                    break;
                case 75: // <Expression_Factor> ::= <Expression_Factor> == <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.Equal));
                    break;
                case 76: // <Expression_Factor> ::= <Expression_Factor> '!=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.NotEqual));
                    break;
                case 77: // <Expression_Binary> ::= <Expression_Unary>
                    // !!! DO NOTHING !!!
                    break;
                case 78: // <Expression_Binary> ::= <Expression_Binary> && <Expression_Unary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(),
                        BinaryOperatorType.And));
                    break;
                case 79: // <Expression_Binary> ::= <Expression_Binary> '||' <Expression_Unary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(
                        new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Or));
                    break;
                case 80: // <Expression_Unary> ::= '+' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Positive));
                    break;
                case 81: // <Expression_Unary> ::= '-' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Negative));
                    break;
                case 82: // <Expression_Unary> ::= '!' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Not));
                    break;
                case 83: // <Expression_Unary> ::= <Expression_Primary>
                    // !!! DO NOTHING !!!
                    break;
                case 84: // <Expression_Unary> ::= <Expression_Primary> '[' <Expression> ']'
                    // Create unary expression.
                    stack.Pop(1);
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(stack.PopExpression(), stack.PopExpression(),
                        UnaryOperatorType.Indexer));
                    break;
                case 85: // <Expression_Primary> ::= <Name>
                    // Create name expression.
                    stack.Push(new Name(stack.PopString()));
                    break;
                case 86: // <Expression_Primary> ::= <Function_Call>
                    // !!! DO NOTHING !!!
                    break;
                case 87: // <Expression_Primary> ::= <Literal>
                    // !!! DO NOTHING !!!
                    break;
                case 88: // <Expression_Primary> ::= '(' <Expression> ')'
                    // Remove pharanthesis
                    stack.Pop(1);
                    stack.Remove(1);
                    break;
            }
        }
    }


    public class ZLanguageVisitor : ZSharpParserBaseVisitor<object>
    {
        private SyntaxStack _syntaxStack;

        public override object VisitModuleDefinition(ZSharpParser.ModuleDefinitionContext context)
        {
            _syntaxStack.Remove(2);
            object result = new Module(_syntaxStack.PopMethodBody(), _syntaxStack.PopString());

            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitModuleBody(ZSharpParser.ModuleBodyContext context)
        {
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
            _syntaxStack.Push(context.IDENTIFIER().GetText());

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
            _syntaxStack.Pop(1);
            _syntaxStack.Push(result);

            return result;
        }

        public override object VisitArrayType(ZSharpParser.ArrayTypeContext context)
        {
            base.VisitArrayType(context);

            _syntaxStack.Pop(2);
            var result = Type.CreateArrayFromType(_syntaxStack.PopType());
            _syntaxStack.Push(result);


            return result;
        }

        public override object VisitLiteral(ZSharpParser.LiteralContext context)
        {
            object result = null;
            if (context.string_literal() != null) result = new Literal(context.GetText(), LiteralType.String);
            else if (context.boolean_literal() != null) result = new Literal(context.GetText(), LiteralType.Boolean);
            else if (context.INTEGER_LITERAL() != null) result = new Literal(context.GetText(), LiteralType.Integer);
            else if (context.REAL_LITERAL() != null) result = new Literal(context.GetText(), LiteralType.Real);
            else if (context.CHARACTER_LITERAL() != null)
                result = new Literal(context.GetText(), LiteralType.Character);

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

            _syntaxStack.Push(result);

            return result;
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
        }


        public override object VisitArguments(ZSharpParser.ArgumentsContext context)
        {
            _syntaxStack.Push(new ArgumentCollection());
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
            _syntaxStack.Push(new Call((ArgumentCollection) _syntaxStack.Pop(), _syntaxStack.PopString()));
            return null;
        }

        public override object VisitStatements(ZSharpParser.StatementsContext context)
        {
            _syntaxStack.Push(new StatementCollection());
            return null;
        }

        public override object VisitStatement(ZSharpParser.StatementContext context)
        {
            base.VisitStatement(context);

            object result;

            if (context.RETURN() != null)
                if (context.expression() == null)
                    result = new Return(null);
                else
                    result = new Return(_syntaxStack.PopExpression());
            else if (context.expression() != null)
                _syntaxStack.PeekCollection().Add(_syntaxStack.PopExpression());
        }
    }
}