

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the schema of the table whose column is to be renamed
    /// </summary>
    public interface IRenameColumnToOrInSchemaSyntax : IRenameColumnToSyntax
    {
        /// <summary>
        /// Specify the schema name
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IRenameColumnToSyntax InSchema(string schemaName);
    }
}
