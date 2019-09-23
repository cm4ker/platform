

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the table schema or a new column
    /// </summary>
    public interface ICreateTableWithColumnOrSchemaSyntax : ICreateTableWithColumnSyntax
    {
        /// <summary>
        /// Define the tables schema
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Define a new column</returns>
        ICreateTableWithColumnSyntax InSchema(string schemaName);
    }
}
