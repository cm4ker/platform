using System;
using Aquila.QueryBuilder.Common.Factoryes;
using Aquila.QueryBuilder.Common.Operations;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.QueryBuilder.DML.Select;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common.Columns
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