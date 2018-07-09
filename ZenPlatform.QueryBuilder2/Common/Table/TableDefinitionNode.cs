using System;
using System.Linq;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Table
{
    public class TableDefinitionNode : Node
    {
        public TableDefinitionNode()
        {

        }

        public ColumnDefinitionNode WithColumn(string columnName)
        {
            if (Childs.Any())
                Add(Tokens.Tokens.CommaToken);

            var col = new ColumnDefinitionNode(columnName);
            Add(col);
            return col;
        }

        public TableDefinitionNode WithColumn(string columnName, Func<TypeDefinitionFactory, TypeDefinitionNode> type)
        {
            WithColumn(columnName).WithType(type);
            return this;
        }
    }
}