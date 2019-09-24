

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The root expression for a DELETE operation
    /// </summary>
    public interface IDeleteExpressionRoot 
    {
        /// <summary>
        /// Specify the schema to delete
        /// </summary>
        /// <param name="schemaName">The name of the schema</param>
        void Schema(string schemaName);

        /// <summary>
        /// Specify the table to delete
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IInSchemaSyntax Table(string tableName);

        /// <summary>
        /// Specify the column to delete
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>The next step</returns>
        IDeleteColumnFromTableSyntax Column(string columnName);

        /// <summary>
        /// Specify that a foreign key with a default name should be deleted
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteForeignKeyFromTableSyntax ForeignKey();

        /// <summary>
        /// Specify that a foreign key with the given name should be deleted
        /// </summary>
        /// <param name="foreignKeyName">The foreign key name</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyOnTableSyntax ForeignKey(string foreignKeyName);

        /// <summary>
        /// Deletes data from a table
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IDeleteDataOrInSchemaSyntax FromTable(string tableName);

        /// <summary>
        /// Deletes an index
        /// </summary>
        /// <param name="indexName">the name of the index</param>
        /// <returns>The next step</returns>
        IDeleteIndexForTableSyntax Index(string indexName);

        /// <summary>
        /// Deletes an index, based on the naming convention in effect
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteIndexForTableSyntax Index();

        /// <summary>
        /// Delete a sequence with the given name
        /// </summary>
        /// <param name="sequenceName">The sequence name</param>
        /// <returns>The next step</returns>
        IInSchemaSyntax Sequence(string sequenceName);

        /// <summary>
        /// Deletes a named Primary Key from a table
        /// </summary>
        /// <param name="primaryKeyName">The name of the primary key</param>
        /// <returns>The next step</returns>
        IDeleteConstraintOnTableSyntax PrimaryKey(string primaryKeyName);

        /// <summary>
        /// Deletes a named Unique Constraint From a table
        /// </summary>
        /// <param name="constraintName">The constraint name</param>
        /// <returns>The next step</returns>
        IDeleteConstraintOnTableSyntax UniqueConstraint(string constraintName);

        /// <summary>
        /// Deletes a named Unique Constraint from a table based on the naming convention in effect
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteConstraintOnTableSyntax UniqueConstraint();

        /// <summary>
        /// Deletes a default constraint from a column
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteDefaultConstraintOnTableSyntax DefaultConstraint();
    }
}
