

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the new table name or schema of the table to rename
    /// </summary>
    public interface IRenameTableToOrInSchemaSyntax 
    {
        /// <summary>
        /// Specify the tables schema name
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IRenameTableToSyntax InSchema(string schemaName);
    }
}
