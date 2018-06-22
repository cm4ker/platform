namespace ZenPlatform.QueryBuilder2.DML.From
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