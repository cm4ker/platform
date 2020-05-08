using System;
using Aquila.QueryBuilder.Common.Factoryes;
using Aquila.QueryBuilder.Common.SqlTokens;

namespace Aquila.QueryBuilder.Common.Conditions
{
    /// <summary>
    /// Фабрика условных выражений
    /// </summary>
    public class ConditionFactory
    {
        private static ConditionFactory _instance = new ConditionFactory();

        private ConditionFactory()
        {
        }

        public static ConditionFactory Get()
        {
            return _instance;
        }

        public LikeConditionNode Like(Func<SqlNodeFactory, SqlNode> expr, Func<SqlNodeFactory, SqlNode> pattern)
        {
            return new LikeConditionNode(expr(SqlNodeFactory.Get()), pattern(SqlNodeFactory.Get()));
        }

        public UnaryConditionNode Not(Func<SqlNodeFactory, SqlNode> expr)
        {
            var node = expr(SqlNodeFactory.Get());

            return new UnaryConditionNode(node);
        }

        public IsNullConditionNode IsNull(Func<SqlNodeFactory, SqlNode> expr)
        {
            var node = expr(SqlNodeFactory.Get());

            return new IsNullConditionNode(node);
        }

        public BinaryConditionNode Condition(Func<SqlNodeFactory, SqlNode> expr1, ComparerToken token, Func<SqlNodeFactory, SqlNode> expr2)
        {
            return new BinaryConditionNode(expr1(SqlNodeFactory.Get()), token, expr2(SqlNodeFactory.Get()));
        }

        public AndConditionNode And(Func<ConditionFactory, ConditionNode[]> expr)
        {
            return new AndConditionNode(expr(_instance));
        }

        public OrConditionNode Or(Func<ConditionFactory, ConditionNode[]> expr)
        {
            return new OrConditionNode(expr(_instance));
        }
    }

    /// <summary>
    /// Выражает оператор AND
    /// </summary>
    public class AndConditionNode : ConditionNode
    {
        public AndConditionNode(params SqlNode[] nodes)
        {
            Childs.AddRange(nodes);
        }
    }

    /// <summary>
    /// Выражает оператор OR
    /// </summary>
    public class OrConditionNode : ConditionNode
    {
        public OrConditionNode(params SqlNode[] nodes)
        {
            Childs.AddRange(nodes);
        }

    }
}