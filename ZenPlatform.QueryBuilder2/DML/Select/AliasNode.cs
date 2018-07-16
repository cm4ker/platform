using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class AliasNode : SqlNode
    {
//        public string Alias { get; }

        public AliasNode(string alias)
        {
            Childs.Add(new IdentifierNode(alias));
            //Alias = alias;
        }
    }
}