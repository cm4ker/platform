using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class AliasedTableNode : SqlNode
    {
        private TableNode _table;

        public AliasedTableNode(string tableName)
        {
            _table = new TableNode(tableName);
            Childs.Add(_table);
        }

        public AliasedTableNode As(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                Add(new AliasNode(alias));

            return this;
        }

        public AliasedTableNode WithSchema(string schemaName)
        {
            _table.WithSchema(schemaName);
            return this;
        }
    }
}