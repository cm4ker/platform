using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class TopNode : Node
    {
        public TopNode(int count)
        {
            Childs.Add(Tokens.TopToken);
            Childs.Add(Tokens.SpaceToken);
            Childs.Add(new RawSqlNode(count.ToString()));
            Childs.Add(Tokens.SpaceToken);
        }
    }
}