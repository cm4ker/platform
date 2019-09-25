

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the schema of the foreign keys primary table
    /// </summary>
    public interface ICreateForeignKeyPrimaryColumnOrInSchemaSyntax : ICreateForeignKeyPrimaryColumnSyntax
    {
        /// <summary>
        /// Specify the schema of the foreign keys primary table
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Specify the foreign keys primary table columns</returns>
        ICreateForeignKeyPrimaryColumnSyntax InSchema(string schemaName);
    }
}
