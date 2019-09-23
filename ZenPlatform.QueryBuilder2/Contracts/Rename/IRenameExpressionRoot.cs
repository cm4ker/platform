

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The expression root for renaming tables or columns
    /// </summary>
    public interface IRenameExpressionRoot 
    {
        /// <summary>
        /// Specify the table (or its column) to be renamed
        /// </summary>
        /// <param name="oldName">The current table name</param>
        /// <returns>The next step</returns>
        IRenameTableToOrInSchemaSyntax Table(string oldName);

        /// <summary>
        /// Specify the column to be renamed
        /// </summary>
        /// <param name="oldName">The current column name</param>
        /// <returns>The next step</returns>
        IRenameColumnTableSyntax Column(string oldName);
    }
}
