

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the schema or foreign key column to delete
    /// </summary>
    public interface IDeleteForeignKeyForeignColumnOrInSchemaSyntax : IDeleteForeignKeyForeignColumnSyntax
    {
        /// <summary>
        /// Define the schema
        /// </summary>
        /// <param name="foreignSchemaName">The schema of the foreign key</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyForeignColumnSyntax InSchema(string foreignSchemaName);
    }
}
