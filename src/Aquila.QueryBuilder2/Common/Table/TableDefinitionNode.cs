using System;
using System.Linq;
using Aquila.QueryBuilder.Common.Columns;
using Aquila.QueryBuilder.Common.Factoryes;
using Aquila.QueryBuilder.Common.Operations;
using Aquila.QueryBuilder.Common.SqlTokens;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.Common.Table
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