

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Definition of the schema the table belongs to
    /// </summary>
    public interface ICreateIndexOnColumnOrInSchemaSyntax : ICreateIndexOnColumnSyntax
    {
        /// <summary>
        /// Defines the schema of the table to create the index for
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Definition of index columns</returns>
        ICreateIndexOnColumnSyntax InSchema(string schemaName);
    }
}
