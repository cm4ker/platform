using System;
using System.Linq;
using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;

namespace ZenPlatform.QueryBuilder2.DML.From
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