using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
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