namespace ZenPlatform.QueryBuilder2.From
{
    public class JoinNode : SqlNode
    {
        public JoinType JoinType { get; }

        public JoinNode(SqlNode joinObject, JoinType joinType)
        {
            Childs.Add(joinObject);
            JoinType = joinType;
        }

        public void On(string tableName, string fieldName, string condition, string tableName2, string fieldName2)
        {
            Childs.Add(new OnNode(tableName, fieldName, condition, tableName2, fieldName2));
        }
    }
}