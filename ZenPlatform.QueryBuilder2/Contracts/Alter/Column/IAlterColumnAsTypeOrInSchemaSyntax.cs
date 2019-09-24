

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface to fluently specify the table schema of the column modification
    /// </summary>
    public interface IAlterColumnAsTypeOrInSchemaSyntax : IAlterColumnAsTypeSyntax
    {
        /// <summary>
        /// Specify the table schema of the column modification
        /// </summary>
        /// <param name="schemaName">The table schema name</param>
        /// <returns>The interface for the column modification</returns>
        IAlterColumnAsTypeSyntax InSchema(string schemaName);
    }
}
