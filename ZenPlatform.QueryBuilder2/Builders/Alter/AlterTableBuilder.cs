using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    public class AlterTableBuilder : ExpressionBuilderWithColumnTypesBase<IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax>,
                                               IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax,
                                               IAlterTableColumnAsTypeSyntax,
                                               IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax
    {
        private AlterTableNode _alterTable;
        private ColumnDefinitionNode _currentColumn;
        public AlterTableBuilder()
        {
            _alterTable = new AlterTableNode();
        }
        
        public IAlterTableColumnAsTypeSyntax AddColumn(string name)
        {
            _currentColumn = new ColumnDefinitionNode();
            _currentColumn.Column = new ColumnNode(name);
            _alterTable.AddColumns.Add(_currentColumn);
            return this;
        }

        public IAlterTableColumnAsTypeSyntax AlterColumn(string name)
        {
            _currentColumn = new ColumnDefinitionNode();
            _currentColumn.Column = new ColumnNode(name);
            _alterTable.AlterColumns.Add(_currentColumn);
            return this;
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey(string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey(string foreignKeyName, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey(string foreignKeyName, string primaryTableSchema, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ForeignKey()
        {
            throw new NotImplementedException();
        }

        public override ColumnDefinitionNode GetColumnForType()
        {
            return _currentColumn;
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Identity()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Indexed()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Indexed(string indexName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableAddColumnOrAlterColumnSyntax InSchema(string schemaName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax NotNullable()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Nullable()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax OnDelete(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax OnDeleteOrUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax OnUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax PrimaryKey()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax PrimaryKey(string primaryKeyName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ReferencedBy(string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ReferencedBy(string foreignKeyName, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnOrForeignKeyCascadeSyntax ReferencedBy(string foreignKeyName, string foreignTableSchema, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax SetExistingRowsTo(object value)
        {
            throw new NotImplementedException();
        }

        public void ToSchema(string name)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Unique()
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax Unique(string indexName)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax WithColumnDescription(string description)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax WithDefault(SystemMethods method)
        {
            throw new NotImplementedException();
        }

        public IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax WithDefaultValue(object value)
        {
            throw new NotImplementedException();
        }

        public IAlterTableAddColumnOrAlterColumnOrSchemaSyntax WithDescription(string description)
        {
            throw new NotImplementedException();
        }
    }

    internal class AlterTableNode
    {
    }
}
