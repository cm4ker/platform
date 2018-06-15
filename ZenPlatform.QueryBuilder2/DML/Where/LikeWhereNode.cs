using ZenPlatform.QueryBuilder2.Select;

namespace ZenPlatform.QueryBuilder2
{
    public class LikeWhereNode : WhereExpression
    {
        public SqlNode Expression { get; }
        public SqlNode Pattern { get; }

        public LikeWhereNode(SqlNode expression, SqlNode pattern)
        {
            Expression = expression;
            Pattern = pattern;

            Childs.AddRange(new[] {expression, new RawSqlNode(" LIKE "), pattern});
        }
    }
}