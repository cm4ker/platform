

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the schema or the condition of the data to delete
    /// </summary>
    public interface IDeleteDataOrInSchemaSyntax : IDeleteDataSyntax
    {
        /// <summary>
        /// Specify the schema of the table to delete from
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IDeleteDataSyntax InSchema(string schemaName);
    }
}
