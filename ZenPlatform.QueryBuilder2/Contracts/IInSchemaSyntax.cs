

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Defines the schema
    /// </summary>
    public interface IInSchemaSyntax 
    {
        /// <summary>
        /// Specifies the schema
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        void InSchema(string schemaName);
    }
}
