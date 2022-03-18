using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Contracts;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class AlterColumnBuilder : ExpressionBuilderWithColumnOptionsAndTypesBase<AlterColumnBuilder>
    {
        private AlterAddColumn _alterColumn;

        public AlterColumnBuilder(AlterAddColumn alterColumn)
        {
            _alterColumn = alterColumn;
        }


        public AlterColumnBuilder Column(ColumnDefinition column)
        {
            _alterColumn.Column = column;

            return this;
        }

        public AlterColumnBuilder Column(string columnName)
        {
            _alterColumn.Column = new ColumnDefinition()
            {
                Column = new Column() { Value = columnName }
            };

            return this;
        }


        public override ColumnDefinition GetCurrentColumn()
        {
            return _alterColumn.Column;
        }

        public override void SetType(ColumnType columnType)
        {
            _alterColumn.Column.Type = columnType;
        }


        public AlterColumnBuilder OnTable(string tableName)
        {
            _alterColumn.Table = new Table() { Value = tableName };
            return this;
        }

        public override void SetConstraintDefinition(ConstraintDefinition constraint)
        {
            throw new NotImplementedException();
        }
    }
}