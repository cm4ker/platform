using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class StringLiteralNode : SqlNode
    {
        public string RawString { get; }

        public StringLiteralNode(string rawString)
        {
            RawString = rawString;
        }
    }
}