using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class LikeWhereNode : WhereExpression
    {
        public Node Expression { get; }
        public Node Pattern { get; }

        public LikeWhereNode(SqlNode expression, SqlNode pattern)
        {
            Expression = expression;
            Pattern = pattern;

            Childs.AddRange(new[] {expression, new RawSqlNode(" LIKE "), pattern});
        }
    }
}