

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Extension point for unique column options
    /// </summary>
    public interface ICreateIndexColumnUniqueOptionsSyntax : ICreateIndexOnColumnSyntax
    {
        /// <summary>
        /// Access to the current index column definition
        /// </summary>
        //IndexColumnDefinition CurrentColumn { get; }
    }
}
