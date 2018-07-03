using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common
{
    public class RawSqlNode : SqlNode
    {
        public RawSqlNode(string raw)
        {
            Raw = raw;
        }

        public string Raw { get; set; }
    }
}