using System;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DML.From
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
            Childs.Add(Tokens.SpaceToken);
            Childs.Add(option(fac));
        }
    }
}