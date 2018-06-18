namespace ZenPlatform.QueryBuilder2.From
{
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
}