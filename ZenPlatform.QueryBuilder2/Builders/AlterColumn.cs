using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class AlterColumnBuilder : ExpressionBuilderWithColumnOptionsAndTypesBase<AlterColumnBuilder>
    {
        private Expression _expression;
        private AlterColumn _alterColumnExpression;
        
        public AlterColumnBuilder(string columnName)
        {
            

            var column = new ColumnDefinition()
            {
                Column = new Column() { Value = columnName }
            };

            _alterColumnExpression = new AlterColumn()
            {
                Column = column
            };

            _expression = new Expression();
            _expression.Add(_alterColumnExpression);

        }

        public QuerySyntaxNode Expression => _expression;

        public override ColumnDefinition GetCurrentColumn()
        {
            return _alterColumnExpression.Column;
        }

        public override void SetConstraintDefinition(ConstraintDefinition constraint)
        {
            

            _expression.Nodes.Add(new AddConstraint()
            {
                Constraint = constraint,
                Table = _alterColumnExpression.Table
            });
        }

        public override void SetType(ColumnType columnType)
        {
            _alterColumnExpression.Column.Type = columnType;
        }


        public AlterColumnBuilder OnTable(string tableName)
        {
            _alterColumnExpression.Table = new Table() { Value = tableName };
            return this;
        }

    }
}
