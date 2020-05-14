using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common
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