using System.Data;

namespace ZenPlatform.QueryBuilder.Common.Conditions
{
    public class UnaryConditionNode : ConditionExpression
    {
        public UnaryConditionNode(SqlNode node)
        {
            Childs.Add(node);
        }
    }
}