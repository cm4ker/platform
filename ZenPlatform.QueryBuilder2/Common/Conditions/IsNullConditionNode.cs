namespace ZenPlatform.QueryBuilder.Common.Conditions
{
    /// <summary>
    /// Выражение условия Is null
    /// </summary>
    public class IsNullConditionNode : ConditionExpression
    {
        public IsNullConditionNode(SqlNode node)
        {
            Childs.Add(node);
        }
    }
}