using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Expressions
{
    /// <summary>
    /// Binary expression
    /// </summary>
    public class BinaryExpression : Expression
    {
        private const int RIGHT_SLOT = 0;
        private const int LEFT_SLOT = 1;

        private Expression _right;
        private Expression _left;
        private BinaryOperatorType _binaryOperatorType;

        public Expression Right
        {
            get => _right ?? Children.GetSlot(out _right, RIGHT_SLOT);
            set => _right = Children.SetSlot(value, RIGHT_SLOT);
        }

        public Expression Left
        {
            get => _left ?? Children.GetSlot(out _left, LEFT_SLOT);
            set => _left = Children.SetSlot(value, LEFT_SLOT);
        }

        public BinaryOperatorType BinaryOperatorType
        {
            get => _binaryOperatorType;
            set => _binaryOperatorType = value;
        }

        public BinaryExpression(ILineInfo li, Expression right, Expression left,
            BinaryOperatorType binaryOperatorType) : base(li)
        {
            _right = Children.SetSlot(right, RIGHT_SLOT);
            _left = Children.SetSlot(left, LEFT_SLOT);
            _binaryOperatorType = binaryOperatorType;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }

        public override TypeNode Type
        {
            get
            {
                TypeNode tn;

                switch (_binaryOperatorType)
                {
                    case BinaryOperatorType.Or:
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.GreaterThen:
                    case BinaryOperatorType.Equal:
                    case BinaryOperatorType.NotEqual:
                    case BinaryOperatorType.LessThen:
                    case BinaryOperatorType.LessOrEqualTo:
                    case BinaryOperatorType.GraterOrEqualTo:
                        tn = new PrimitiveTypeNode(null, TypeNodeKind.Boolean);
                        break;
                    default:
                        tn = Left.Type;
                        break;
                }

                return tn;
            }
        }
    }
}