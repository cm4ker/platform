

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the data or schema to insert
    /// </summary>
    public interface IInsertDataOrInSchemaSyntax : IInsertDataSyntax
    {
        /// <summary>
        /// Specify the schema of the table to insert data
        /// </summary>
        /// <param name="schemaName">The schema</param>
        /// <returns>The next step</returns>
        IInsertDataSyntax InSchema(string schemaName);
    }
}
