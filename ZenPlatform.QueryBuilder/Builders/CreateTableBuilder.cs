using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class CreateTableBuilder : ExpressionBuilderWithColumnOptionsAndTypesBase<CreateTableBuilder>
    {
        private CreateTable _createTable;
        private ColumnDefinition _currentColumn;


        public CreateTableBuilder(string tableName)
        {
            _createTable = new CreateTable()
            {
                Table = new Table() {Value = tableName}
            };
        }

        public CreateTableBuilder(CreateTable createTable)
        {
            _createTable = createTable;
        }

        public CreateTableBuilder WithColumnDefinition(ColumnDefinition column)
        {
            _createTable.Columns.Add(column);
            return this;
        }

        public CreateTableBuilder WithColumn(Action<ColumnDefinitionBuilder> action)
        {
            ColumnDefinitionBuilder builder = new ColumnDefinitionBuilder();

            action(builder);

            _createTable.Columns.Add(builder.ColumnDefinition);
            return this;
        }

        public CreateTableBuilder WithColumn(string columnName)
        {
            _currentColumn = new ColumnDefinition()
            {
                Column = new Column() {Value = columnName}
            };

            _createTable.Columns.Add(_currentColumn);

            return this;
        }

        public CreateTableBuilder CheckExists()
        {
            _createTable.CheckExists = true;
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