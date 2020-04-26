namespace ZenPlatform.QueryBuilder.Common
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