using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Contracts;

namespace ZenPlatform.QueryBuilder.Builders.Create
{
    public class CreateExpressionRoot : ICreateExpressionRoot
    {

        /// <inheritdoc />
        public ICreateSchemaOptionsSyntax Schema(string schemaName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateTableWithColumnOrSchemaOrDescriptionSyntax Table(string tableName)
        {

            return new CreateTableBuilder(tableName);
        }

        /// <inheritdoc />
        public ICreateColumnOnTableSyntax Column(string columnName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateForeignKeyFromTableSyntax ForeignKey()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateForeignKeyFromTableSyntax ForeignKey(string foreignKeyName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateIndexForTableSyntax Index()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateIndexForTableSyntax Index(string indexName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateSequenceInSchemaSyntax Sequence(string sequenceName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateConstraintOnTableSyntax UniqueConstraint()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateConstraintOnTableSyntax UniqueConstraint(string constraintName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateConstraintOnTableSyntax PrimaryKey()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICreateConstraintOnTableSyntax PrimaryKey(string primaryKeyName)
        {
            throw new NotImplementedException();
        }
    }
}
