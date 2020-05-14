using System;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common.Operations
{
    public class TypeDefinitionNode : SqlNode
    {
        public TypeDefinitionNode(string typeName)
        {
            Childs.Add(new IdentifierNode(typeName));
        }

        private bool _hasNullableTag;

        public TypeDefinitionNode NotNull()
        {
            if (_hasNullableTag) throw new Exception();
            _hasNullableTag = true;
            Childs.AddRange(Tokens.SpaceToken, Tokens.NotToken, Tokens.SpaceToken, Tokens.NullToken);
            return this;
        }

        public TypeDefinitionNode Null()
        {
            if (_hasNullableTag) throw new Exception();
            _hasNullableTag = true;
            Childs.AddRange(Tokens.SpaceToken, Tokens.NullToken);
            return this;
        }

        public TypeDefinitionNode Unique()
        {
            Childs.AddRange(Tokens.SpaceToken, Tokens.UniqueToken);
            return this;
        }
    }
}