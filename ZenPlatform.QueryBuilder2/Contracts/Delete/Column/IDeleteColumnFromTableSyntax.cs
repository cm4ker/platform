

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Delete a column from a table
    /// </summary>
    public interface IDeleteColumnFromTableSyntax 
    {
        /// <summary>
        /// Define the table to delete the column from
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IInSchemaSyntax FromTable(string tableName);

        /// <summary>
        /// Delete define the column to delete
        /// </summary>
        /// <param name="columnName">The name of the column to delete</param>
        /// <returns>The next step</returns>
        IDeleteColumnFromTableSyntax Column(string columnName);
    }
}
