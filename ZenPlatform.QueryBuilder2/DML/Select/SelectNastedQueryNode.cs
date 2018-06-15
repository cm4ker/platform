namespace ZenPlatform.QueryBuilder2.Select
{
    public class SelectNastedQueryNode : SqlNode
    {
        public SelectNastedQueryNode(SelectQueryNode node)
        {
            Childs.Add(node);
        }

        public void As(string alias)
        {
            Childs.Add(new AliasNode(alias));
        }
    }
}