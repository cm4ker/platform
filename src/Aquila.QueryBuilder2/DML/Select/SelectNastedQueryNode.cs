using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
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