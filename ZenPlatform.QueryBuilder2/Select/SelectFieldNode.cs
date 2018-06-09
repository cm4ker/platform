namespace ZenPlatform.QueryBuilder2
{
    public class SelectFieldNode : SqlNode, IAliasedNode
    {
        public SelectFieldNode(string fieldName)
        {
            FieldName = fieldName;
        }

        public SelectFieldNode(string fieldName, string alias) : this(fieldName)
        {
            Alias = alias;
        }

        public SelectFieldNode(string fieldName, string alias, SqlNode parentSource) : this(fieldName, alias)
        {
            ParentSource = parentSource;
        }

        public string FieldName { get; set; }
        public string Alias { get; set; }

        public bool IsAliased => !string.IsNullOrEmpty(Alias);
        public bool HasParentSource => ParentSource != null;

        public SqlNode ParentSource { get; set; }
    }
}