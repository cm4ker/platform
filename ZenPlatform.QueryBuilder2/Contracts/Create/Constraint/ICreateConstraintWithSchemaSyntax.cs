

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the constraint schema
    /// </summary>
    public interface ICreateConstraintWithSchemaSyntax 
    {
        /// <summary>
        /// The constraint schema
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>Specify the constraint columns</returns>
        ICreateConstraintColumnsSyntax WithSchema(string schemaName);
    }
}
