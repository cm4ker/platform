using Aquila.QueryBuilder.Common;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.From
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