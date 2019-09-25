

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the options or table of the index to delete
    /// </summary>
    public interface IDeleteIndexForTableSyntax 
    {
        /// <summary>
        /// Specify the table of the index to delete
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IDeleteIndexOnColumnOrInSchemaSyntax OnTable(string tableName);

        /// <summary>
        /// Specify the options of the index to delete
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteIndexOptionsSyntax WithOptions();
    }
}
