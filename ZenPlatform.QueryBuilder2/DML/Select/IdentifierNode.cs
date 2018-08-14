using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
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