

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the column or columns of the index to dlete
    /// </summary>
    public interface IDeleteIndexOnColumnSyntax 
    {
        /// <summary>
        /// Specify the column of the index to delete
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>The next step</returns>
        IDeleteIndexOptionsSyntax OnColumn(string columnName);

        /// <summary>
        /// Specify the columns of the index to delete
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns>The next step</returns>
        IDeleteIndexOptionsSyntax OnColumns(params string[] columnNames);

        /// <summary>
        /// Specify the options of the index to delete
        /// </summary>
        /// <returns>The next step</returns>
        IDeleteIndexOptionsSyntax WithOptions();
    }
}
