

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Interface the specify the update condition
    /// </summary>
    public interface IUpdateWhereSyntax 
    {
        /// <summary>
        /// Specify the condition of the rows to update
        /// </summary>
        /// <param name="dataAsAnonymousType">The columns and values to be used as condition</param>
        void Where(object dataAsAnonymousType);

        /// <summary>
        /// Specify that all rows should be updated
        /// </summary>
        void AllRows();
    }
}
