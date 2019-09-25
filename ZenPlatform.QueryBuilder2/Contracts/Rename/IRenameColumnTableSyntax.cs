

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the table (or its column) to be renamed
    /// </summary>
    public interface IRenameColumnTableSyntax 
    {
        /// <summary>
        /// Specify the name of the table (or its column) to be renamed
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IRenameColumnToOrInSchemaSyntax OnTable(string tableName);
    }
}
