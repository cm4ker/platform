using System.Linq;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Table
{
    public class TableWithColumnsNode : Node
    {
        private ColumnListNode _columnList;
        private TableNode _table;

        public TableWithColumnsNode(string tableName)
        {
            _columnList = new ColumnListNode();
            _table = new TableNode(tableName);

            Childs.Add(_table);
            Childs.Add(Tokens.Tokens.LeftBracketToken);
            Childs.Add(_columnList);
            Childs.Add(Tokens.Tokens.RightBracketToken);
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
            if (_columnList.Childs.Any())
            {
                _columnList.Add(Tokens.Tokens.CommaToken);
                _columnList.Add(Tokens.Tokens.SpaceToken);
            }

            _columnList.Add(node);

            return this;
        }
    }
}