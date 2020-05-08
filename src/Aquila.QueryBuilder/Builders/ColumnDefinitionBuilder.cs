using System;
using System.Collections.Generic;
using System.Text;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Builders
{
    public class ColumnDefinitionBuilder: ExpressionBuilderWithColumnOptionsAndTypesBase<ColumnDefinitionBuilder>
    {
        private readonly ColumnDefinition _columnDefinition;

        public ColumnDefinitionBuilder()
        {
            _columnDefinition = new ColumnDefinition();
        }

        public ColumnDefinitionBuilder WithColumnName(string columnName)
        {
            _columnDefinition.Column = new Column() { Value = columnName };
            return this;
        }

        public ColumnDefinition ColumnDefinition => _columnDefinition;

        public override void SetType(ColumnType columnType)
        {
            _columnDefinition.Type = columnType;
        }

        public override void SetConstraintDefinition(ConstraintDefinition constraint)
        {
            throw new NotImplementedException();

            
        }

        public override ColumnDefinition GetCurrentColumn()
        {
            return _columnDefinition;
        }
    }
}
