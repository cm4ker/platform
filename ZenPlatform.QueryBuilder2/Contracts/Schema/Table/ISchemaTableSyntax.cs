

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Queries a tables (or one of its childs) existence
    /// </summary>
    public interface ISchemaTableSyntax
    {
        /// <summary>
        /// Returns <c>true</c> when the table exists
        /// </summary>
        /// <returns><c>true</c> when the table exists</returns>
        bool Exists();

        /// <summary>
        /// Specifies the column to check
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>The next step</returns>
        ISchemaColumnSyntax Column(string columnName);

        /// <summary>
        /// Specify the index to check
        /// </summary>
        /// <param name="indexName">The index name</param>
        /// <returns>The next step</returns>
        ISchemaIndexSyntax Index(string indexName);

        /// <summary>
        /// Specify the constraint to check
        /// </summary>
        /// <param name="constraintName">The constraint name</param>
        /// <returns>The next step</returns>
        ISchemaConstraintSyntax Constraint(string constraintName);
    }
}
