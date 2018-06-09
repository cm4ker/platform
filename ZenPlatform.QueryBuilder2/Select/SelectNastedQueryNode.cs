namespace ZenPlatform.QueryBuilder2
{
    public class SelectNastedQueryNode : SqlNode, IAliasedNode
    {
        public SelectNastedQueryNode(SelectQueryNode node, string alias)
        {
            Childs.Add(node);
            Alias = alias;
        }

        public string Alias { get; }
        public bool IsAliased { get; }
    }
}