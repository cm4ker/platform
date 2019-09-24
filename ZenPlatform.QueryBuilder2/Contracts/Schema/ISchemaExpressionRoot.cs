

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The expression root to query the existence of database objects
    /// </summary>
    public interface ISchemaExpressionRoot 
    {
        /// <summary>
        /// Specify the table as base to query a database objects existence
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        ISchemaTableSyntax Table(string tableName);

        /// <summary>
        /// Specify the schema as base to query a database objects existence
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        ISchemaSchemaSyntax Schema(string schemaName);
    }
}
