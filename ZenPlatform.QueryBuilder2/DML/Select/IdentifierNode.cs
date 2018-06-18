using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class IdentifierNode : SqlNode
    {
        public string Name { get; }

        public IdentifierNode(string name)
        {
            Name = name;
        }
    }
}