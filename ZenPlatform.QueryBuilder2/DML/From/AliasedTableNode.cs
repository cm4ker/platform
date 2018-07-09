using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.From
{
    public class AliasedTableNode : Node
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