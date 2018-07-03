using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common.Tokens
{
    public class Token : SqlNode
    {
        public Token(string name)
        {
            Childs.Add(new RawSqlNode(name));
        }
    }
}