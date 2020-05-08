using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common.Conditions
{
    public class LikeConditionNode : ConditionNode
    {
        public SqlNode Expression { get; }
        public SqlNode Pattern { get; }

        public LikeConditionNode(SqlNode expression, SqlNode pattern)
        {
            Expression = expression;
            Pattern = pattern;

            Childs.AddRange(new[] {expression, pattern});
        }
    }
}