

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Delete the default constraint from the given column
    /// </summary>
    public interface IDeleteDefaultConstraintOnColumnOrInSchemaSyntax : IDeleteDefaultConstraintOnColumnSyntax
    {
        /// <summary>
        /// The table schema of the columns default constraint
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns>The next step</returns>
        IDeleteDefaultConstraintOnColumnSyntax InSchema(string schemaName);
    }
}
