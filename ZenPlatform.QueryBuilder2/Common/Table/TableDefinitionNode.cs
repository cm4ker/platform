using System;
using System.Linq;
using ZenPlatform.QueryBuilder2.Common.Columns;
using ZenPlatform.QueryBuilder2.Common.Factoryes;
using ZenPlatform.QueryBuilder2.Common.Operations;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder2.Common.Table
{
    public class TableDefinitionNode : SqlNode
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