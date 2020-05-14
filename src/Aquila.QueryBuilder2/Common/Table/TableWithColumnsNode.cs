using System.Linq;
using Aquila.QueryBuilder.Common.Columns;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common.Table
{
    public class TableWithColumnsNode : SqlNode
    {
        private ColumnListNode _columnList;
        private TableNode _table;

        public TableWithColumnsNode(string tableName)
        {
            _columnList = new ColumnListNode();
            _table = new TableNode(tableName);

            Childs.Add(_table);
            Childs.Add(_columnList);
        }

        public TableWithColumnsNode WithSchema(string schemaName)
        {
            _table.WithSchema(schemaName);
            return this;
        }

        public TableWithColumnsNode WithColumn(string columnName)
        {
            return WithColumn(new ColumnNode(columnName));
        }

        public TableWithColumnsNode WithColumn(ColumnNode node)
        {
            _columnList.Add(node);
            return this;
        }
    }
}