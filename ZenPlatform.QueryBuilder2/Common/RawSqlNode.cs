using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common
{
    public class RawSqlNode : Node
    {
        public RawSqlNode(string raw)
        {
            Raw = raw;
        }

        public string Raw { get; set; }
    }
}