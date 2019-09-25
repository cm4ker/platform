

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to specify the schema or alter a table/column
    /// </summary>
    public interface IAlterTableAddColumnOrAlterColumnOrSchemaSyntax : IAlterTableAddColumnOrAlterColumnSyntax
    {
        /// <summary>
        /// Specify the schema name
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The interface to alter a table/column</returns>
        IAlterTableAddColumnOrAlterColumnSyntax InSchema(string schemaName);
    }
}
