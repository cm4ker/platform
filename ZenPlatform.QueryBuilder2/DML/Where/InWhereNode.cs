using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class InWhereNode : Node
    {
        public InWhereNode(Node node1, Node node2)
        {
            Childs.AddRange(new[] {node1, node2});
        }
    }
}