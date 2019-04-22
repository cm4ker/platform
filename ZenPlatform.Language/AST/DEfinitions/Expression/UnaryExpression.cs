using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.AST.Definitions.Expression
{
    public class UnaryExpression : Infrastructure.Expression
    {
        public Infrastructure.Expression Value = null;
        public Infrastructure.Expression Indexer = null;
        public Type Cast = null;
        public UnaryOperatorType UnaryOperatorType = UnaryOperatorType.None;

//        public UnaryExpression(Infrastructure.Expression indexer, Infrastructure.Expression value,
//            UnaryOperatorType unaryOperatorType)
//        {
//            Value = value;
//            Indexer = indexer;
//            UnaryOperatorType = unaryOperatorType;
//        }
    }


    public class CastExpression : UnaryExpression
    {
        public Infrastructure.Expression Value { get; }
        public Type CastType { get; }

        public CastExpression(Infrastructure.Expression value, Type castType)
        {
            Value = value;
            CastType = castType;
        }
    }

    public class IndexerExpression : UnaryExpression
    {
        public Infrastructure.Expression Indexed { get; }
        public Infrastructure.Expression Value { get; }

        public IndexerExpression(Infrastructure.Expression indexed, Infrastructure.Expression value)
        {
            Indexed = indexed;
            Value = value;
        }
    }

    public class LogicalOrArithmeticExpression : UnaryExpression
    {
        public Infrastructure.Expression Value { get; }
        public UnaryOperatorType Type { get; }

        public LogicalOrArithmeticExpression(Infrastructure.Expression value, UnaryOperatorType type)
        {
            Value = value;
            Type = type;
        }
    }
}