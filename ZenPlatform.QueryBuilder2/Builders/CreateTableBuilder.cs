using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class CreateTableBuilder : ExpressionBuilderWithColumnOptionsAndTypesBase<CreateTableBuilder>, IExpression
    {
        private CreateTable _createTable;
        private ColumnDefinition _currentColumn;

        public QuerySyntaxNode Expression => _createTable;

        public CreateTableBuilder(string tableName)
        {
            _createTable = new CreateTable()
            {
                Table = new Table() { Value = tableName }
            };


            
        }

        public CreateTableBuilder WithColumn(string columnName)
        {
            _currentColumn = new ColumnDefinition()
            {
                Column = new Column() { Value = columnName }
            };

            _createTable.Columns.Add(_currentColumn);

            return this;
        }

        public override void SetType(ColumnType columnType)
        {
            _currentColumn.Type = columnType;
        }

        public override void SetConstraintDefinition(ConstraintDefinition constraint)
        {
            _createTable.Constraints.Add(constraint);
        }

        public override ColumnDefinition GetCurrentColumn()
        {
            return _currentColumn;
        }
    }
}
