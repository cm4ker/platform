namespace Aquila.QueryBuilder.Common.Conditions
{
    public class InConditionNode : SqlNode
    {
        public InConditionNode(SqlNode node1, SqlNode node2)
        {
            Childs.AddRange(new[] {node1, node2});
        }
    }
}