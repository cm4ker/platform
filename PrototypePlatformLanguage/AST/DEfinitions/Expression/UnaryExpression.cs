using PrototypePlatformLanguage.AST.Infrastructure;

namespace PrototypePlatformLanguage.AST.Definitions.Expression
{
    public class UnaryExpression : Infrastructure.Expression
    {
        public Infrastructure.Expression Value = null;
        public Infrastructure.Expression Indexer = null;
        public UnaryOperatorType UnaryOperatorType = UnaryOperatorType.None;

        public UnaryExpression(Infrastructure.Expression indexer, Infrastructure.Expression value, UnaryOperatorType unaryOperatorType)
        {
            Value = value;
            Indexer = indexer;
            UnaryOperatorType = unaryOperatorType;
        }
    }
}