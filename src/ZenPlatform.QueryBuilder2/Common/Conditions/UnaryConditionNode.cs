using System.Data;

namespace ZenPlatform.QueryBuilder.Common.Conditions
{
    public class UnaryConditionNode : ConditionNode
    {
        public UnaryConditionNode(SqlNode node)
        {
            Childs.Add(node);
        }
    }
}