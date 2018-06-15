namespace ZenPlatform.QueryBuilder2
{
    public class SelectFieldNode : SqlNode
    {
        public SelectFieldNode(string fieldName)
        {
            Childs.Add(new IdentifierNode(fieldName));
        }

        public SelectFieldNode(string fieldName, string alias) : this(fieldName)
        {
            if (!string.IsNullOrEmpty(alias))
                Childs.Add(new AliasNode(alias));
        }

        public SelectFieldNode(string tableName, string fieldName, string alias) : this(fieldName, alias)
        {
            Childs.Insert(0, new IdentifierNode(tableName));
            Childs.Insert(1, new SchemaSeparatorNode());
        }
    }

    public class AliasNode : SqlNode
    {
//        public string Alias { get; }

        public AliasNode(string alias)
        {
            Childs.Add(new IdentifierNode(alias));
            //Alias = alias;
        }
    }

    public class IdentifierNode : SqlNode
    {
        public string Name { get; }

        public IdentifierNode(string name)
        {
            Name = name;
        }
    }

    public class SchemaSeparatorNode : SqlNode
    {
        public SchemaSeparatorNode()
        {
        }
    }
}