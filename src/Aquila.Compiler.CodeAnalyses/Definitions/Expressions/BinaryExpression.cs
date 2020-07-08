using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Language.Ast.Definitions.Expressions
{
    public partial class New
    {
        private Ast.TypeSyntax _type;

        public override Ast.TypeSyntax Type
        {
            get
            {
                if (_type == null)
                {
                    _type = new Ast.SingleTypeSyntax(null, Namespace ?? "" + Call.Name.Value, TypeNodeKind.Type);
                    Attach(Type);
                }

                return _type;
            }
        }
    }

    /// <summary>
    /// Binary expression
    /// </summary>
    public partial class BinaryExpression
    {
        public override Ast.TypeSyntax Type
        {
            get
            {
                Ast.TypeSyntax tn;

                switch (BinaryOperatorType)
                {
                    case BinaryOperatorType.Or:
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.GreaterThen:
                    case BinaryOperatorType.Equal:
                    case BinaryOperatorType.NotEqual:
                    case BinaryOperatorType.LessThen:
                    case BinaryOperatorType.LessOrEqualTo:
                    case BinaryOperatorType.GraterOrEqualTo:
                        tn = new Ast.PrimitiveTypeSyntax(null, TypeNodeKind.Boolean);
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