using ZenPlatform.QueryBuilder2.Select;

namespace ZenPlatform.QueryBuilder2.From
{
    public class FromNode : SqlNode
    {
    }

    public class TableNode : SqlNode
    {
        public TableNode(string tableName)
        {
            Childs.Add(new IdentifierNode(tableName));
        }

        public TableNode As(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                Add(new AliasNode(alias));

            return this;
        }

        public TableNode WithSchema(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
            {
                Childs.Insert(0, new IdentifierNode(schemaName));
                Childs.Insert(1, new SchemaSeparatorNode());
            }

            return this;
        }
    }

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

    public class CompareOperatorNode : SqlNode
    {
        public CompareOperatorNode(string op)
        {
            Childs.Add(new RawSqlNode(op));
        }
    }

    public class OnNode : SqlNode
    {
        public OnNode(string tableName, string fieldName, string condition, string tableName2, string fieldName2)
        {
            Childs.AddRange(new SqlNode[]
            {
                new IdentifierNode(tableName),
                new SchemaSeparatorNode(),
                new IdentifierNode(fieldName),
                new CompareOperatorNode(condition),
                new IdentifierNode(tableName2),
                new SchemaSeparatorNode(),
                new IdentifierNode(fieldName2),
            });
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