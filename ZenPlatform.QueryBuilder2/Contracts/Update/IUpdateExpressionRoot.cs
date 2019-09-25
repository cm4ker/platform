

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The expression root to update rows
    /// </summary>
    public interface IUpdateExpressionRoot 
    {
        /// <summary>
        /// Specifies the table to update its data
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IUpdateSetOrInSchemaSyntax Table(string tableName);
    }
}
