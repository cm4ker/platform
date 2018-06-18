using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DML.Where
{
    public class InWhereNode : SqlNode
    {
        public InWhereNode(SqlNode node1, SqlNode node2)
        {
            Childs.AddRange(new[] {node1, node2});
        }
    }
}