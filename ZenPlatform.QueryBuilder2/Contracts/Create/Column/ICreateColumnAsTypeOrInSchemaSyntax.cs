

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Set the column type or the table schema
    /// </summary>
    public interface ICreateColumnAsTypeOrInSchemaSyntax : ICreateColumnAsTypeSyntax
    {
        /// <summary>
        /// Set the table schema
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The interface to define the column type</returns>
        ICreateColumnAsTypeSyntax InSchema(string schemaName);
    }
}
