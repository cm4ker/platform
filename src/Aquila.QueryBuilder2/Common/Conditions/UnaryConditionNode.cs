using System.Data;

namespace Aquila.QueryBuilder.Common.Conditions
{
    public class UnaryConditionNode : ConditionNode
    {
        public UnaryConditionNode(SqlNode node)
        {
            Childs.Add(node);
        }
    }
}