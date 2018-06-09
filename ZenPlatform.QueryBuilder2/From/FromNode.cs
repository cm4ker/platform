namespace ZenPlatform.QueryBuilder2
{
    public class FromNode : SqlNode
    {
    }

    public class TableNode : SqlNode, IAliasedNode
    {
        public TableNode(string tableName)
        {
            TableName = tableName;
        }

        public TableNode(string schema, string tableName) : this(tableName)
        {
            Schema = schema;
        }

        public TableNode(string schema, string tableName, string alias) : this(schema, tableName)
        {
            Alias = alias;
        }

        public string Schema { get; set; }
        public string TableName { get; set; }

        public string Alias { get; set; }
        public bool IsAliased => !string.IsNullOrEmpty(Alias);
    }

    public class JoinNode : SqlNode
    {
        public JoinType JoinType { get; }

        public JoinNode(SqlNode joinObject, JoinType joinType)
        {
            Childs.Add(joinObject);
            JoinType = joinType;
        }
    }

    public enum JoinType
    {
        Inner,
        Left,
        Right,
        Full,
        Cross,
        CrossApply
    }
}