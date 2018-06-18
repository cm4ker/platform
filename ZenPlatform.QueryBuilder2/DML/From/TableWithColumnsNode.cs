using ZenPlatform.QueryBuilder2.Select;

namespace ZenPlatform.QueryBuilder2.From
{
    public class TableWithColumnsNode : SqlNode
    {
        private ColumnListNode _columnList;

        public TableWithColumnsNode(string tableName)
        {
            _columnList = new ColumnListNode();

            Childs.Add(new IdentifierNode(tableName));
            Childs.Add(new OpenBraketNode());
            Childs.Add(_columnList);
            Childs.Add(new CloseBracketNode());
        }

        public TableWithColumnsNode WithSchema(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
            {
                Childs.Insert(0, new IdentifierNode(schemaName));
                Childs.Insert(1, new SchemaSeparatorNode());
            }

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