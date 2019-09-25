

namespace ZenPlatform.QueryBuilder.Contracts
{
    /// <summary>
    /// Query an index' existence
    /// </summary>
    public interface ISchemaIndexSyntax
    {
        /// <summary>
        /// Returns <c>true</c> when the index exists
        /// </summary>
        /// <returns><c>true</c> when the index exists</returns>
        bool Exists();
    }
}
