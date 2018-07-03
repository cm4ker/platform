namespace ZenPlatform.QueryBuilder2.Common.Operations
{
    public class BigintTypeDefinitionNode : TypeDefinitionNode
    {
        public BigintTypeDefinitionNode() : base("bigint")
        {
        }
    }

    public class GuidTypeDefinitionNode : TypeDefinitionNode
    {
        public GuidTypeDefinitionNode() : base("uniqueidentifier")
        {
        }
    }
}