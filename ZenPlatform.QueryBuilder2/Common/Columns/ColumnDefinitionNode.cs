using System;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Columns
{
    public class ColumnDefinitionNode : Node
    {
        public ColumnDefinitionNode(string columnName)
        {
            Childs.Add(new IdentifierNode(columnName));
        }

        public void WithType(Func<TypeDefinitionFactory, TypeDefinitionNode> option)
        {
            var fac = new TypeDefinitionFactory();
            Childs.Add(Tokens.Tokens.SpaceToken);
            Childs.Add(option(fac));
        }
    }
}