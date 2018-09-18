using System;
using ZenPlatform.QueryBuilder.Common.Factoryes;

namespace ZenPlatform.QueryBuilder.Common.Conditions
{
    public class ConditionFactory
    {
        private static ConditionFactory _instance = new ConditionFactory();

        private ConditionFactory()
        {
        }

        public static ConditionFactory Get() => _instance;


        public LikeConditionNode Like(Func<SqlNodeFactory, SqlNode> expr, Func<SqlNodeFactory, SqlNode> pattern)
        {
            return new LikeConditionNode(expr(SqlNodeFactory.Get()), pattern(SqlNodeFactory.Get()));
        }

        public UnaryConditionNode Not(Func<SqlNodeFactory, SqlNode> expr)
        {
            var node = expr(SqlNodeFactory.Get());

            return new UnaryConditionNode(node);
        }

        public IsNullConditionNode IsNull()
        {
            return new IsNullConditionNode();
        }
    }
}