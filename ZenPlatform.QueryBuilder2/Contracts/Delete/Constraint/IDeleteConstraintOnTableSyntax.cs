

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Delete the constraint from the given table
    /// </summary>
    public interface IDeleteConstraintOnTableSyntax 
    {
        /// <summary>
        /// Specify the table to delete the constraint from
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IDeleteConstraintInSchemaOptionsSyntax FromTable(string tableName);
    }
}
