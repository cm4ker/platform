

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the column to delete the default constraint from
    /// </summary>
    public interface IDeleteDefaultConstraintOnColumnSyntax 
    {
        /// <summary>
        /// The column to delete the default constraint from
        /// </summary>
        /// <param name="columnName">The column name</param>
        void OnColumn(string columnName);
    }
}
