

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// The root of the INSERT expression
    /// </summary>
    public interface IInsertExpressionRoot 
    {
        /// <summary>
        /// Specify the table name to insert data to
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IInsertDataOrInSchemaSyntax IntoTable(string tableName);
    }
}
