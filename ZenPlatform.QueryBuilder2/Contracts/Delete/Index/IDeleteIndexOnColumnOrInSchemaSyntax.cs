

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the column or schema of the index to delete
    /// </summary>
    public interface IDeleteIndexOnColumnOrInSchemaSyntax 
    {
        /// <summary>
        /// The schema of the index to delete
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IDeleteIndexOnColumnSyntax InSchema(string schemaName);
    }
}
