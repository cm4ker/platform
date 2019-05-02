using System.Diagnostics;
using Antlr4.Runtime.Atn;
using Mono.Cecil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expression;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Calculation
{
    public class AstVerifier
    {
        private readonly CompilationUnit _cUnit;
        public event VerifierEventHandler Error;


        public AstVerifier(CompilationUnit cUnit)
        {
            _cUnit = cUnit;
        }

        private void BuildSymbolTable(SymbolTable parent, InstructionsBodyNode body)
        {
            if (body.Statements != null)
            {
                foreach (Statement statement in body.Statements)
                {
                    if (statement is Variable)
                        parent.Add(statement as Variable).SyntaxObject = statement;
                }
            }
        }

        public bool VerifyExpression(SymbolTable symbolTable, Infrastructure.Expression expression)
        {
            //
            // Verify expression.
            //

            try
            {
                GetExpressionType(symbolTable, expression);
                return true;
            }
            catch (VerifierException x)
            {
                System.Diagnostics.Debug.WriteLine(x.Message);
                return false;
            }
        }

        public ZType GetExpressionType(SymbolTable symbolTable, Infrastructure.Expression expression)
        {
            if (expression is UnaryExpression unary)
            {
                if (unary is IndexerExpression ie)
                    return FindType(GetExpressionType(symbolTable, unary.Value), UnaryOperatorType.Negative);
                if (unary is LogicalOrArithmeticExpression lae)
                    return FindType(GetExpressionType(symbolTable, unary.Value), lae.Type);
                if (unary is CastExpression ce)
                    return ce.Type;
            }
            else if (expression is BinaryExpression)
            {
                BinaryExpression binary = (BinaryExpression) expression;
                return FindType(GetExpressionType(symbolTable, binary.Left),
                    GetExpressionType(symbolTable, binary.Right), binary.BinaryOperatorType);
            }
            else if (expression is Literal)
            {
                Literal literal = expression as Literal;

                return literal.Type;
            }
            else if (expression is Name)
            {
                return ((Variable) symbolTable.Find(((Name) expression).Value, SymbolType.Variable).SyntaxObject).Type;
            }
            else if (expression is Call)
            {
                return ((Call) expression).Type;
            }

            return null;
        }

        public ZType FindType(ZType leftType, ZType rightType, BinaryOperatorType operatorType)
        {
            //
            // Binary operations can only be performed on primitive types.
            //

            if (!leftType.IsSystem || !rightType.IsSystem)
                throw new VerifierException("Binary operations can only be performed on primitive types.");

            //
            // Boolean operations.
            //

            if (leftType is ZBool && rightType is ZBool)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                    case BinaryOperatorType.NotEqual:
                        return ZTypeSystem.Bool;
                        break;
                    default:
                        throw new VerifierException("Specified operator cannot be applied to boolean types.");
                        break;
                }
            }

            //
            // Integer operations.
            //

            else if (leftType is ZInt && rightType is ZInt)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to integer types.");
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                    case BinaryOperatorType.GreaterThen:
                    case BinaryOperatorType.LessOrEqualTo:
                    case BinaryOperatorType.LessThen:
                    case BinaryOperatorType.Equal:
                    case BinaryOperatorType.NotEqual:
                        return ZTypeSystem.Bool;
                        break;
                    default:
                        return ZTypeSystem.Bool;
                }
            }

            throw new VerifierException("Incompatible types for specified operation.");
        }

        public ZType FindType(ZType type, UnaryOperatorType operatorType)
        {
            switch (operatorType)
            {
                case UnaryOperatorType.Indexer:
                    if (type.IsArray && type is ZArray a)
                        return a.TypeOfElements;
                    else
                        throw new VerifierException("The indexer operator cannot be applied to the specified type.");
                    break;
                case UnaryOperatorType.Not:
                    if (type is ZBool)
                        return type;
                    else
                        throw new VerifierException("The NOT operator cannot be applied to the specified type.");
                    break;
                default:
                    if (type.IsSystem)
                        return type;
                    else
                        throw new VerifierException("The +/- operators cannot be applied to the specified type.");
                    break;
            }
        }

        public bool VerifyBody(InstructionsBodyNode body)
        {
            //
            // Check variable declarations.
            //

            foreach (Statement statement in body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = (Variable) statement;
                    if (variable.Value is Infrastructure.Expression)
                    {
                        if (VerifyExpression(body.SymbolTable, (Infrastructure.Expression) variable.Value))
                            Debug.WriteLine("All right");
                        else
                            Debug.WriteLine("Smth wrong");
                    }
                }
            }

            return false;
        }
    }
}