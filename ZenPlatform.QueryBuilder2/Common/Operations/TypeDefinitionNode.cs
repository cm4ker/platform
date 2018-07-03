using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common.Operations
{
    public class TypeDefinitionNode : SqlNode
    {
        public TypeDefinitionNode(string typeName)
        {
            Childs.Add(new IdentifierNode(typeName));
        }

        public TypeDefinitionNode NotNull()
        {
            Childs.AddRange(Tokens.Tokens.SpaceToken, Tokens.Tokens.NotToken, Tokens.Tokens.SpaceToken, Tokens.Tokens.NullToken);
            return this;
        }

        public TypeDefinitionNode Unique()
        {
            Childs.AddRange(Tokens.Tokens.SpaceToken, Tokens.Tokens.UniqueToken);
            return this;
        }
    }
}