using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.Table;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders.Create
{
    public class CreateTableBuilder : ExpressionBuilderWithColumnTypesBase<ICreateTableColumnOptionOrWithColumnSyntax>,
                                                ICreateTableWithColumnOrSchemaOrDescriptionSyntax,
                                                ICreateTableColumnAsTypeSyntax,
                                                ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax
    {

        private TableDefinitionNode _tableDefinitionNode;
        private ColumnDefinitionNode _currentColumn;
        public CreateTableBuilder(string name)
        {
            _tableDefinitionNode = new TableDefinitionNode();
            _tableDefinitionNode.Table = new TableNode(name);
        }

        public QuerySyntaxNode Build()
        {
            return _tableDefinitionNode;
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey(string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey(string foreignKeyName, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey(string foreignKeyName, string primaryTableSchema, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ForeignKey()
        {
            throw new NotImplementedException();
        }

        public override ColumnDefinitionNode GetColumnForType()
        {
            return _currentColumn;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Identity()
        {
            _currentColumn.Identity = true;
            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Indexed()
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Indexed(string indexName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableWithColumnSyntax InSchema(string schemaName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax NotNullable()
        {
            _currentColumn.NotNull = true;
            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Nullable()
        {
            _currentColumn.Null = true;
            return this;
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax OnDelete(Rule rule)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax OnDeleteOrUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax OnUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax PrimaryKey()
        {
            var _currentConstraint = new ConstraintDefinitionNode($"pk_{_currentColumn.Column.Name}_constraint");
            _currentConstraint.Columns.Add(_currentColumn.Column);
            _currentConstraint.Type = new PrimaryKeyTypeConstraintNode();
            _tableDefinitionNode.Constraints.Add(_currentConstraint);

            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax PrimaryKey(string primaryKeyName)
        {
            var _currentConstraint = new ConstraintDefinitionNode(primaryKeyName);
            _currentConstraint.Columns.Add(_currentColumn.Column);
            _currentConstraint.Type = new PrimaryKeyTypeConstraintNode();
            _tableDefinitionNode.Constraints.Add(_currentConstraint);

            return this;
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ReferencedBy(string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ReferencedBy(string foreignKeyName, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax ReferencedBy(string foreignKeyName, string foreignTableSchema, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Unique()
        {
            var _currentConstraint = new ConstraintDefinitionNode($"unique_{_currentColumn.Column.Name}_constraint");
            _currentConstraint.Columns.Add(_currentColumn.Column);
            _currentConstraint.Type = new UniqueTypeConstraintNode();
            _tableDefinitionNode.Constraints.Add(_currentConstraint);

            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax Unique(string indexName)
        {
            var _currentConstraint = new ConstraintDefinitionNode(indexName);
            _currentConstraint.Columns.Add(_currentColumn.Column);
            _currentConstraint.Type = new UniqueTypeConstraintNode();
            _tableDefinitionNode.Constraints.Add(_currentConstraint);

            return this;
        }

        public ICreateTableColumnAsTypeSyntax WithColumn(string name)
        {
            _currentColumn = new ColumnDefinitionNode();
            _currentColumn.Column = new ColumnNode(name);
            _tableDefinitionNode.Columns.Add(_currentColumn);
            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax WithColumnDescription(string description)
        {
            throw new NotImplementedException();
        }

        public ICreateTableColumnOptionOrWithColumnSyntax WithDefault(SystemMethods method)
        {
            _currentColumn.DefaultMethod = method;
            return this;
        }

        public ICreateTableColumnOptionOrWithColumnSyntax WithDefaultValue(object value)
        {
            _currentColumn.DefaultValue = value;
            return this;
        }

        public ICreateTableWithColumnOrSchemaSyntax WithDescription(string description)
        {
            throw new NotImplementedException();
        }
    }
}
