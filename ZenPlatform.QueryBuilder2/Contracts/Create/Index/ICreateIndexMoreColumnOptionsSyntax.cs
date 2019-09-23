

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Extension point for additional index column options
    /// </summary>
    public interface ICreateIndexMoreColumnOptionsSyntax : ICreateIndexOnColumnSyntax
    {
        /// <summary>
        /// Access to the current index column definition
        /// </summary>
        //IndexColumnDefinition CurrentColumn { get; }
    }
}
