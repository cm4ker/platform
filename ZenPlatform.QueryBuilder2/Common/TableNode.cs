using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class TableNode : SqlNode
    {
        public TableNode(string tableName)
        {
            Childs.Add(new IdentifierNode(tableName));
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
}