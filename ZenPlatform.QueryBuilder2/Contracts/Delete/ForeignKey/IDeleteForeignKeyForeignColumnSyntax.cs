

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the foreign key columns to delete
    /// </summary>
    public interface IDeleteForeignKeyForeignColumnSyntax 
    {
        /// <summary>
        /// Specify the column of the foreign key to delete
        /// </summary>
        /// <param name="column">The column name</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyToTableSyntax ForeignColumn(string column);

        /// <summary>
        /// Specify the columns of the foreign key to delete
        /// </summary>
        /// <param name="columns">The foreign keys column names</param>
        /// <returns>The next step</returns>
        IDeleteForeignKeyToTableSyntax ForeignColumns(params string[] columns);
    }
}
