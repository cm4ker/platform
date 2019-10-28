using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class CreateColumnBuilder : ExpressionBuilderWithColumnOptionsAndTypesBase<CreateTableBuilder>, IExpression
    {

        private AddColumn _addColumn;
        private ColumnDefinition _column;
        public CreateColumnBuilder(string columnName)
        {
            _column = new ColumnDefinition()
            {
                Column = new Column() { Value = columnName }
            };

            _addColumn = new AddColumn()
            {
                Column = _column
            };
        }

        public CreateColumnBuilder(ColumnDefinition column)
        {
            _column = column;

            _addColumn = new AddColumn()
            {
                Column = _column
            };
        }



        public QuerySyntaxNode Expression => _addColumn;

        public override ColumnDefinition GetCurrentColumn()
        {
            return _column;
        }

        public override void SetConstraintDefinition(ConstraintDefinition constraint)
        {
            throw new NotImplementedException();
        }

        public override void SetType(ColumnType columnType)
        {
            _column.Type = columnType;
        }
    

        public CreateColumnBuilder OnTable(string tableName)
        {
            _addColumn.Table = new Table() { Value = tableName };
            return this;
        }

    }
}
