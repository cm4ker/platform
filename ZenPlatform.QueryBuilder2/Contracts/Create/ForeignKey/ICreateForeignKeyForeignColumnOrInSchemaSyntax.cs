

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to define the foreign key columns or the foreign keys table schema
    /// </summary>
    public interface ICreateForeignKeyForeignColumnOrInSchemaSyntax : ICreateForeignKeyForeignColumnSyntax
    {
        /// <summary>
        /// Specify the schema of the foreign key table
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Specify the foreign key columns</returns>
        ICreateForeignKeyForeignColumnSyntax InSchema(string schemaName);
    }
}
