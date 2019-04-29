using System.Diagnostics;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expression;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Calculation
{
    public class Verifier
    {
        public event VerifierEventHandler Error;

        private Module _module = null;

        public Verifier(Module module)
        {
            _module = module;
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

        public Type GetExpressionType(SymbolTable symbolTable, Infrastructure.Expression expression)
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

        public Type FindType(Type leftType, Type rightType, BinaryOperatorType operatorType)
        {
            //
            // Binary operations can only be performed on primitive types.
            //

            if ((leftType.VariableType != VariableType.Primitive) || (rightType.VariableType != VariableType.Primitive))
                throw new VerifierException("Binary operations can only be performed on primitive types.");

            //
            // Boolean operations.
            //

            if (leftType.PrimitiveType == PrimitiveType.Boolean && rightType.PrimitiveType == PrimitiveType.Boolean)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                    case BinaryOperatorType.NotEqual:
                        return new Type(PrimitiveType.Boolean);
                        break;
                    default:
                        throw new VerifierException("Specified operator cannot be applied to boolean types.");
                        break;
                }
            }

            //
            // Integer operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Integer &&
                     rightType.PrimitiveType == PrimitiveType.Integer)
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
                        return new Type(PrimitiveType.Boolean);
                        break;
                    default:
                        return new Type(PrimitiveType.Integer);
                }
            }

            //
            // Real operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Real && rightType.PrimitiveType == PrimitiveType.Real)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to real types.");
                        break;
                    default:
                        return new Type(PrimitiveType.Real);
                }
            }

            //
            // Character operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Character &&
                     rightType.PrimitiveType == PrimitiveType.Character)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to character types.");
                        break;
                    default:
                        return new Type(PrimitiveType.Character);
                }
            }

            throw new VerifierException("Incompatible types for specified operation.");
        }

        public Type FindType(Type type, UnaryOperatorType operatorType)
        {
            switch (operatorType)
            {
                case UnaryOperatorType.Indexer:
                    if (type.VariableType == VariableType.PrimitiveArray)
                        return new Type(type.PrimitiveType);
                    else if (type.VariableType == VariableType.StructureArray)
                        return new Type(type.Name);
                    else
                        throw new VerifierException("The indexer operator cannot be applied to the specified type.");
                    break;
                case UnaryOperatorType.Not:
                    if (type.PrimitiveType == PrimitiveType.Boolean)
                        return new Type(type.PrimitiveType);
                    else
                        throw new VerifierException("The NOT operator cannot be applied to the specified type.");
                    break;
                default:
                    if (type.VariableType == VariableType.Primitive)
                        return new Type(type.PrimitiveType);
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