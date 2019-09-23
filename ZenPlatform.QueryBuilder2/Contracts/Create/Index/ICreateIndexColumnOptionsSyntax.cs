

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Setting the default index column options
    /// </summary>
    public interface ICreateIndexColumnOptionsSyntax 
    {
        /// <summary>
        /// Mark the index column as ascending
        /// </summary>
        /// <returns>More column options</returns>
        ICreateIndexMoreColumnOptionsSyntax Ascending();

        /// <summary>
        /// Mark the index column as descending
        /// </summary>
        /// <returns>More column options</returns>
        ICreateIndexMoreColumnOptionsSyntax Descending();

        /// <summary>
        /// Mark the index column as unique
        /// </summary>
        /// <returns>More column options for the unique column</returns>
        ICreateIndexColumnUniqueOptionsSyntax Unique();
    }
}
