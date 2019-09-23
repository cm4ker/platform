

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the schema or the data to update
    /// </summary>
    public interface IUpdateSetOrInSchemaSyntax : IUpdateSetSyntax
    {
        /// <summary>
        /// Specify the schema of the table to update its data
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IUpdateSetSyntax InSchema(string schemaName);
    }
}
