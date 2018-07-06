using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class InWhereNode : SqlNode
    {
        public InWhereNode(SqlNode node1, SqlNode node2)
        {
            Childs.AddRange(new[] {node1, node2});
        }
    }
}