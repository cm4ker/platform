using System;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.Common.Operations;
using ZenPlatform.QueryBuilder2.DML.Select;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common.Columns
{
    public class ColumnDefinitionNode : SqlNode
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