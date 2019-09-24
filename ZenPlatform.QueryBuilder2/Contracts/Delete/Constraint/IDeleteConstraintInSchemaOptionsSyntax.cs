

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the schema of the constraint to delete
    /// </summary>
    public interface IDeleteConstraintInSchemaOptionsSyntax : IDeleteConstraintColumnSyntax
    {
        /// <summary>
        /// Define the schema of the constraint to delete
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IDeleteConstraintInSchemaOptionsSyntax InSchema(string schemaName);
    }
}
