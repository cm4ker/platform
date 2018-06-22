using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
{
    public class TypeDefinitionNode : SqlNode
    {
        public TypeDefinitionNode(string typeName)
        {
            Childs.Add(new IdentifierNode(typeName));
        }

        public TypeDefinitionNode NotNull()
        {
            Childs.AddRange(Tokens.SpaceToken, Tokens.NotToken, Tokens.SpaceToken, Tokens.NullToken);
            return this;
        }

        public TypeDefinitionNode Unique()
        {
            Childs.AddRange(Tokens.SpaceToken, Tokens.UniqueToken);
            return this;
        }
    }
}