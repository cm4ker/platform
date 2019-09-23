

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Index options
    /// </summary>
    public interface ICreateIndexOptionsSyntax 
    {
        /// <summary>
        /// Defines the index as unique
        /// </summary>
        /// <returns>Defines the column for the index</returns>
        ICreateIndexOnColumnSyntax Unique();

        /// <summary>
        /// Defines the index as non-clustered
        /// </summary>
        /// <returns>Defines the column for the index</returns>
        ICreateIndexOnColumnSyntax NonClustered();

        /// <summary>
        /// Defines the index as clustered
        /// </summary>
        /// <returns>Defines the column for the index</returns>
        ICreateIndexOnColumnSyntax Clustered();
    }
}
