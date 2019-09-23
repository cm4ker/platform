

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the condition of the data to delete
    /// </summary>
    public interface IDeleteDataSyntax 
    {
        /// <summary>
        /// Define the condition of a row/multiple rows to delete
        /// </summary>
        /// <param name="dataAsAnonymousType">An anonymous type whose member names will be trated as column names and their values as values for the condition</param>
        /// <returns>The next step</returns>
        IDeleteDataSyntax Row(object dataAsAnonymousType);

        /// <summary>
        /// Specify that all rows should be deleted
        /// </summary>
        void AllRows();

        /// <summary>
        /// Specify that all rows having a <c>null</c> value in the given column should be deleted
        /// </summary>
        /// <param name="columnName">The column name</param>
        void IsNull(string columnName);
    }
}
