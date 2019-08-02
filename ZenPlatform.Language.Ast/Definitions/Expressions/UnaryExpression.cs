using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions.Expressions
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
    }

    public partial class LogicalOrArithmeticExpression : UnaryExpression
    {
    }
}