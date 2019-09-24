

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Define the table of the column to delete the default constraint from
    /// </summary>
    public interface IDeleteDefaultConstraintOnTableSyntax 
    {
        /// <summary>
        /// Specify the name of the table to delete the columns default constraint from
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <returns>The next step</returns>
        IDeleteDefaultConstraintOnColumnOrInSchemaSyntax OnTable(string tableName);
    }
}
