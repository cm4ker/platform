using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Operations
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