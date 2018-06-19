namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class TypeDefinitionFactory
    {
        public TypeDefinitionNode Varchar()
        {
            return new VarcharTypeDefinitionNode();
        }
        public TypeDefinitionNode DateTime()
        {
            return new DateTimeTypeDefinitionNode();
        }
        public TypeDefinitionNode BinaryArray()
        {
            return new BinaryArrayTypeDefinitionNode();
        }
        public TypeDefinitionNode Boolean()
        {
            return new BooleanTypeDefinitionNode();
        }
        public TypeDefinitionNode Int()
        {
            return new IntTypeDefinitionNode();
        }
        public TypeDefinitionNode Bigint()
        {
            return new BigintTypeDefinitionNode();
        }

    }
}