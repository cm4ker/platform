using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Contracts;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Builders
{
    class AlterColumnBuilder :
        ExpressionBuilderWithColumnTypesBase<IAlterColumnOptionSyntax>,
        IAlterColumnOnTableSyntax,
        IAlterColumnAsTypeOrInSchemaSyntax,
        IAlterColumnOptionOrForeignKeyCascadeSyntax
    {

        private ColumnDefinitionNode _currentColumn;
        public IAlterColumnOptionOrForeignKeyCascadeSyntax ForeignKey(string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ForeignKey(string foreignKeyName, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ForeignKey(string foreignKeyName, string primaryTableSchema, string primaryTableName, string primaryColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ForeignKey()
        {
            throw new NotImplementedException();
        }

        public override ColumnDefinitionNode GetColumnForType()
        {
            return _currentColumn;
        }

        public IAlterColumnOptionSyntax Identity()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax Indexed()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax Indexed(string indexName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnAsTypeSyntax InSchema(string schemaName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax NotNullable()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax Nullable()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax OnDelete(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax OnDeleteOrUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnAsTypeOrInSchemaSyntax OnTable(string name)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax OnUpdate(Rule rule)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax PrimaryKey()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax PrimaryKey(string primaryKeyName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ReferencedBy(string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ReferencedBy(string foreignKeyName, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionOrForeignKeyCascadeSyntax ReferencedBy(string foreignKeyName, string foreignTableSchema, string foreignTableName, string foreignColumnName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax Unique()
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax Unique(string indexName)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax WithColumnDescription(string description)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax WithDefault(SystemMethods method)
        {
            throw new NotImplementedException();
        }

        public IAlterColumnOptionSyntax WithDefaultValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}
