

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Delete a constraint column
    /// </summary>
    public interface IDeleteConstraintColumnSyntax 
    {
        /// <summary>
        /// The name of the column to delete
        /// </summary>
        /// <param name="columnName">The column name</param>
        void Column(string columnName);

        /// <summary>
        /// The names of the columns to delete
        /// </summary>
        /// <param name="columnNames">The column names</param>
        void Columns(params string[] columnNames);
    }
}
