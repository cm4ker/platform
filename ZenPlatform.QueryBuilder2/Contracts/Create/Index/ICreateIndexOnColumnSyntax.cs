

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Definition of index columns or options
    /// </summary>
    public interface ICreateIndexOnColumnSyntax 
    {
        /// <summary>
        /// Defines the index column
        /// </summary>
        /// <param name="columnName">The column name</param>
        /// <returns>Defines the index column options</returns>
        ICreateIndexColumnOptionsSyntax OnColumn(string columnName);

        /// <summary>
        /// Set the index options
        /// </summary>
        /// <returns>Defines the index options</returns>
        ICreateIndexOptionsSyntax WithOptions();
    }
}
