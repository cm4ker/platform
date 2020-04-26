using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
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