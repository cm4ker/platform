

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Specify the data to update
    /// </summary>
    public interface IUpdateSetSyntax 
    {
        /// <summary>
        /// Specify the values to be set
        /// </summary>
        /// <param name="dataAsAnonymousType">The columns and values to be used set</param>
        /// <returns>The next step</returns>
        IUpdateWhereSyntax Set(object dataAsAnonymousType);
    }
}
