using ZenPlatform.QueryBuilder2.Where;

namespace ZenPlatform.QueryBuilder2
{
    public class InWhereNode : SqlNode
    {
        public InWhereNode(SqlNode node1, SqlNode node2)
        {
            Childs.AddRange(new[] {node1, node2});
        }
    }
}