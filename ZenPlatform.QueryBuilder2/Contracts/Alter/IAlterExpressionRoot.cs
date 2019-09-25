



namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The root expression interface for the alterations
    /// </summary>
    public interface IAlterExpressionRoot 
    {
        /// <summary>
        /// Alter the table or its columns/options
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The interface for the modifications</returns>
        IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax Table(string tableName);

        /// <summary>
        /// Alter the column for a given table
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>The interface to specify the table</returns>
        IAlterColumnOnTableSyntax Column(string columnName);
    }
}
