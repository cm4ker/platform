

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Query a schemas (or its of its tables) existence
    /// </summary>
    public interface ISchemaSchemaSyntax 
    {
        /// <summary>
        /// Returns <c>true</c> when a schema exists
        /// </summary>
        /// <returns><c>true</c> when a schema exists</returns>
        bool Exists();

        /// <summary>
        /// Specify the table name as basis for a query a database objects existence
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        ISchemaTableSyntax Table(string tableName);
    }
}
