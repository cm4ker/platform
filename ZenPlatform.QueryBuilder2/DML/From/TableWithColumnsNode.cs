using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
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
            Childs.Add(Tokens.LeftBracketToken);
            Childs.Add(_columnList);
            Childs.Add(Tokens.RightBracketToken);
        }

        public TableWithColumnsNode WithSchema(string schemaName)
        {
            _table.WithSchema(schemaName);
            return this;
        }

        public TableWithColumnsNode WithField(string fieldName)
        {
            return WithField(new FieldNode(fieldName));
        }

        public TableWithColumnsNode WithField(FieldNode node)
        {
            _columnList.Add(node);
            return this;
        }
    }
}