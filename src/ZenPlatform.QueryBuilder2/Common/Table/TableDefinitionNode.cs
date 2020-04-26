using System;
using System.Linq;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.Factoryes;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.Common.Table
{
    public class TableDefinitionNode : SqlNode
    {
        public TableDefinitionNode()
        {

        }

        public ColumnDefinitionNode WithColumn(string columnName)
        {
            if (Childs.Any())
                Add(Tokens.CommaToken);

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