using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
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