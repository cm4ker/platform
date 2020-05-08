namespace Aquila.QueryBuilder.Common.Conditions
{
    /// <summary>
    /// Выражение условия Is null
    /// </summary>
    public class IsNullConditionNode : ConditionNode
    {
        public IsNullConditionNode(SqlNode node)
        {
            Childs.Add(node);
        }
    }
}