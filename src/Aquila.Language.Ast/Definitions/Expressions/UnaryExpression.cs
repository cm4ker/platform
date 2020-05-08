using System;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public abstract partial class UnaryExpression : Expression
    {
        public override TypeSyntax Type => Expression.Type;
    }

    public partial class CastExpression : UnaryExpression
    {
        public override TypeSyntax Type => CastType;
    }

    public partial class IndexerExpression : UnaryExpression
    {
        public override TypeSyntax Type =>
            (Expression.Type as ArrayTypeSyntax ??
             throw new Exception("Expression have to be array type or something is wrong?")).ElementType;
    }

    public partial class LogicalOrArithmeticExpression : UnaryExpression
    {
        public override TypeSyntax Type
        {
            get
            {
                TypeSyntax tn;

                switch (OperaotrType)
                {
                    case UnaryOperatorType.Not:
                        tn = new PrimitiveTypeSyntax(null, TypeNodeKind.Boolean);
                        break;
                    default:
                        tn = Expression.Type;
                        break;
                }


                return tn;
            }
        }
    }
}